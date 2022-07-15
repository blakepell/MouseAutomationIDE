/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using LuaAutomation.Common;
using LuaAutomation.Lua;
using LuaAutomation.ViewModels;
using Microsoft.Win32;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Platforms;
using Rect = LuaAutomation.Common.Rect;

namespace LuaAutomation.Pages
{
    public partial class LuaEditorPage
    {
        /// <summary>
        /// Application Settings
        /// </summary>
        public AppSettings AppSettings { get; set; }

        /// <summary>
        /// The view model for the <see cref="LuaEditorPage"/>.
        /// </summary>
        public LuaEditorViewModel ViewModel { get; set; }

        /// <summary>
        /// Used for auto completion with Lua.
        /// </summary>
        CompletionWindow _completionWindow;

        /// <summary>
        /// Used to cancel scripts.
        /// </summary>
        private ExecutionControlToken _executionControlToken;

        /// <summary>
        /// Whether or not the syntax highlighting has been loaded, for use if Loaded is called multiple
        /// times which seems to be happening when the control is made visible.
        /// </summary>
        private bool _isSyntaxLoaded = false;

        /// <summary>
        /// A reference to the LuaEditorPage this control resides on.
        /// </summary>
        private LuaEditorPage? _luaEditorPage;

        /// <summary>
        /// A list of recorded mouse events.
        /// </summary>
        public List<MouseEvent> MouseEvents = new();

        /// <summary>
        /// A <see cref="Stopwatch"/> used to record the time a mouse event occurred.
        /// </summary>
        private Stopwatch _recorderStopwatch = new();

        /// <summary>
        /// Lua Interpreter
        /// </summary>
        private Script Script { get; set; }

        public LuaEditorPage()
        {
            InitializeComponent();

            // Register our shared objects with the DI container.
            this.ViewModel = new();
            AppServices.AddSingleton(this);
            AppServices.AddSingleton(this.ViewModel);

            // Retrieve DI objects we need.
            this.AppSettings = AppServices.GetRequiredService<AppSettings>();

            // Set the data context to the view model.
            this.DataContext = this.ViewModel;

            if (this.AppSettings.ShowConsoleOnStartup)
            {
                ToggleSwitchConsoleVisible.IsChecked = true;
            }
            else
            {
                ToggleSwitchConsoleVisible.IsChecked = false;
            }

            // Setup Lua, we're going to create instances of our CLR objects that
            // extend Lua.
            UserData.RegisterType<MouseScriptCommands>();

            var mouseCommands = new MouseScriptCommands();

            // Register the type if it's not already registered.
            if (!UserData.IsTypeRegistered(mouseCommands.GetType()))
            {
                UserData.RegisterType(mouseCommands.GetType());
            }

            var uiCommands = new UIScriptCommands();

            // Register the type if it's not already registered.
            if (!UserData.IsTypeRegistered(uiCommands.GetType()))
            {
                UserData.RegisterType(uiCommands.GetType());
            }

            UserData.RegisterType<Rect>();

            // Setup the initial state of button that is not the default.
            this.ViewModel.StopButtonBrush = UIBrushes.GrayBrush;
            this.ViewModel.StopButtonEnabled = false;

            // Create the Lua Interpreter.
            this.Script = new();
            this.Script.Options.CheckThreadAccess = false;
            this.Script.Globals.Set("mouse", UserData.Create(mouseCommands));
            this.Script.Globals.Set("ui", UserData.Create(uiCommands));

            // Warmup the script engine.
            Script.WarmUp();

            // Was there any text was auto saved?
            this.Editor.Text = this.AppSettings.AutoSaveText;

            // Wire up the mouse hooks for recording macros.
            App.MouseHook.MouseMove += MouseHookOnMouseMove;
            App.MouseHook.LeftButtonDown += MouseHookOnLeftButtonDown;
            App.MouseHook.LeftButtonUp += MouseHookOnLeftButtonUp;
            App.MouseHook.RightButtonDown += MouseHookOnRightButtonDown;
            App.MouseHook.RightButtonUp += MouseHookOnRightButtonUp;
            App.MouseHook.MiddleButtonDown += MouseHookOnMiddleButtonDown;
            App.MouseHook.MiddleButtonUp += MouseHookOnMiddleButtonUp;

            // Wire up the key hooks for recording macros.
            App.KeyHook.KeyUp += KeyHookOnKeyUp;
        }

        private void AvalonLuaEditor_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.SetupLuaEditor();

            Editor.TextArea.TextEntering += AvalonLuaEditor_TextEntering;
            Editor.TextArea.TextEntered += AvalonLuaEditor_TextEntered;
        }

        /// <summary>
        /// Fired when the UserControl is unloaded.  Cleanup any resources including removing EventHandlers
        /// so this memory can be properly disposed of.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AvalonLuaEditor_OnUnloaded(object sender, RoutedEventArgs e)
        {
            Editor.TextArea.TextEntering -= AvalonLuaEditor_TextEntering;
            Editor.TextArea.TextEntered -= AvalonLuaEditor_TextEntered;
        }

        /// <summary>
        /// Event that fires when the page is shown like during a navigation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LuaEditorPage_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                this.AppSettings.AutoSaveText = this.Editor.Text;
            }
        }

        /// <summary>
        /// Runs a Lua script in the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonRunLua_OnClick(object sender, RoutedEventArgs e)
        {
            if (!this.ViewModel.PlayButtonEnabled)
            {
                return;
            }

            this.ViewModel.LuaInterpreterStatus = "Running";

            if (AppSettings.ClearConsoleOnRun)
            {
                Console.Clear();
            }

            if (AppSettings.ShowConsoleOnRun)
            {
                ToggleSwitchConsoleVisible.IsChecked = true;
            }

            var luaPage = AppServices.GetRequiredService<LuaEditorPage>();
            _executionControlToken = new();

            this.ViewModel.PlayButtonEnabled = false;
            this.ViewModel.PlayButtonBrush = Brushes.Gray;

            this.ViewModel.RecordButtonEnabled = false;
            this.ViewModel.RecordButtonBrush = Brushes.Gray;

            this.ViewModel.StopButtonEnabled = true;
            this.ViewModel.StopButtonBrush = UIBrushes.RedBrush;

            this.ViewModel.StatusBarForegroundBrush = UIBrushes.WhiteBrush;
            this.ViewModel.StatusBarBackgroundBrush = UIBrushes.GreenBrush;

            var sw = new Stopwatch();

            try
            {
                sw.Start();
                _ = await this.Script.DoStringAsync(_executionControlToken, this.Editor.Text);
                //this.Script.DoString(this.Editor.Text);
                sw.Stop();
                
                luaPage.ViewModel.LuaInterpreterStatus = $"Completed in {sw.ElapsedMilliseconds / 1000}s";

                // Reset the status to default
                this.ViewModel.StatusBarForegroundBrush = UIBrushes.WhiteBrush;
                this.ViewModel.StatusBarBackgroundBrush = UIBrushes.LightBlueBrush;
            }
            catch (Exception ex)
            {
                sw.Stop();

                if (ex.GetBaseException() is ScriptTerminationRequestedException)
                {
                    this.ViewModel.LuaInterpreterStatus = "Stopped";

                    // Reset the status to default
                    this.ViewModel.StatusBarForegroundBrush = UIBrushes.WhiteBrush;
                    this.ViewModel.StatusBarBackgroundBrush = UIBrushes.LightBlueBrush;
                }
                else
                {
                    if (ex.InnerException is InterpreterException luaEx)
                    {
                        // TODO: DI inject
                        var script = new UIScriptCommands();
                        script.ConsoleLog($"ERROR {luaEx.DecoratedMessage}");
                    }
                    else
                    {
                        var script = new UIScriptCommands();
                        script.ConsoleLog($"ERROR {ex.Message}");
                    }

                    this.ViewModel.LuaInterpreterStatus = "Error";

                    // Set the status to error
                    this.ViewModel.StatusBarForegroundBrush = UIBrushes.WhiteBrush;
                    this.ViewModel.StatusBarBackgroundBrush = UIBrushes.RedBrush;
                }

                //luaPage.ViewModel.StatusText = ex.GetBaseException() is ScriptTerminationRequestedException 
                //    ? "Stopped" 
                //    : $"Error: {ex.Message} => Runtime: {sw.ElapsedMilliseconds / 1000}s";
            }
            finally
            {
                this.ViewModel.PlayButtonEnabled = true;
                this.ViewModel.PlayButtonBrush = UIBrushes.GreenBrush;

                this.ViewModel.RecordButtonEnabled = true;
                this.ViewModel.RecordButtonBrush = UIBrushes.RedBrush;

                this.ViewModel.StopButtonEnabled = false;
                this.ViewModel.StopButtonBrush = UIBrushes.GrayBrush;
            }
        }

        /// <summary>
        /// Stops a Lua script that's running.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStop_OnClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.LuaInterpreterStatus = "Stopping";

            _executionControlToken?.Terminate();

            this.ViewModel.PlayButtonEnabled = false;
            this.ViewModel.PlayButtonBrush = UIBrushes.GrayBrush;

            this.ViewModel.RecordButtonEnabled = false;
            this.ViewModel.RecordButtonBrush = UIBrushes.GrayBrush;

            this.ViewModel.StopButtonEnabled = false;
            this.ViewModel.StopButtonBrush = UIBrushes.GrayBrush;

            this.ViewModel.StatusBarForegroundBrush = UIBrushes.WhiteBrush;
            this.ViewModel.StatusBarBackgroundBrush = UIBrushes.OrangeBrush;

            if (!this._recorderStopwatch.IsRunning)
            {
                return;
            }

            this._recorderStopwatch.Stop();

            this.ViewModel.StatusText = $"{this.MouseEvents.Count:N0} events recorded.";

            for (int i = this.MouseEvents.Count - 1; i > 0; i--)
            {
                var ts = this.MouseEvents[i].TimeSpan - this.MouseEvents[i - 1].TimeSpan;
                this.MouseEvents[i].DelayMilliseconds = Convert.ToInt32(ts.Value.TotalMilliseconds);
            }

            var sb = new StringBuilder();

            foreach (var ev in this.MouseEvents)
            {
                switch (ev.EventType)
                {
                    case MouseEventType.MouseMove:
                        sb.Append($"mouse.SetPosition({ev.X}, {ev.Y})\r\n");
                        break;
                    case MouseEventType.LeftDown:
                        sb.Append("mouse.LeftDown()\r\n");
                        break;
                    case MouseEventType.LeftUp:
                        sb.Append("mouse.LeftUp()\r\n");
                        break;
                    case MouseEventType.RightDown:
                        sb.Append("mouse.RightDown()\r\n");
                        break;
                    case MouseEventType.RightUp:
                        sb.Append("mouse.RightUp()\r\n");
                        break;
                    case MouseEventType.MiddleDown:
                        sb.Append("mouse.MiddleDown()\r\n");
                        break;
                    case MouseEventType.MiddleUp:
                        sb.Append("mouse.MiddleUp()\r\n");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // Put the delay so it simulates close to the time frame the user used
                // when recording it.
                if (ev.DelayMilliseconds > 0)
                {
                    sb.Append($"ui.Sleep({ev.DelayMilliseconds})\r\n");
                }
            }

            this.Editor.Text = sb.ToString();

            // Reenable the menu options.
            this.ViewModel.PlayButtonEnabled = true;
            this.ViewModel.PlayButtonBrush = UIBrushes.GreenBrush;

            this.ViewModel.RecordButtonEnabled = true;
            this.ViewModel.RecordButtonBrush = UIBrushes.RedBrush;

            this.ViewModel.StopButtonEnabled = false;
            this.ViewModel.StopButtonBrush = UIBrushes.GrayBrush;
        }

        /// <summary>
        /// Sets up the syntax highlighting for the interactive Lua editor.
        /// </summary>
        public void SetupLuaEditor()
        {
            // If the syntax has already been loaded, get out.
            if (_isSyntaxLoaded)
            {
                return;
            }

            try
            {
                LoadSyntaxHighlighting(Editor);
            }
            catch (Exception ex)
            {

            }

            _isSyntaxLoaded = true;
        }

        /// <summary>
        /// Loads the syntax highlighting rules, both from the stored resource and dynamically.
        /// </summary>
        /// <param name="te"></param>
        /// <remarks>
        /// This was made a static method so that other instances of that require our Lua syntax highlighting
        /// but might not be using this control can reference this code.
        /// </remarks>
        public static void LoadSyntaxHighlighting(TextEditor te)
        {
            var asm = Assembly.GetExecutingAssembly();

            using (var s = asm.GetManifestResourceStream($"{asm.GetName().Name}.Resources.LuaDarkTheme.xshd"))
            {
                using (var reader = new XmlTextReader(s))
                {
                    te.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

                    // Dynamic syntax highlighting for your own purpose
                    var rules = te.SyntaxHighlighting.MainRuleSet.Rules;

                    var rule = new HighlightingRule
                    {
                        Color = new HighlightingColor()
                        {
                            Foreground = new CustomizedBrush((Color)ColorConverter.ConvertFromString(("#DCDCAA")))
                        }
                    };

                    // Construct our custom highlighting rule via reflection.
                    var t = typeof(MouseScriptCommands);
                    var ui = typeof(UIScriptCommands);

                    var sb = StringBuilderPool.Take();
                    sb.Append(@"\b(");

                    // This should get all of our methods but exclude ones that are defined on
                    // object like ToString, GetHashCode, Equals, etc.
                    foreach (var method in t.GetMethods().Where(m => m.DeclaringType != typeof(object)))
                    {
                        sb.AppendFormat("{0}|", method.Name);
                    }

                    foreach (var method in ui.GetMethods().Where(m => m.DeclaringType != typeof(object)))
                    {
                        sb.AppendFormat("{0}|", method.Name);
                    }

                    sb.TrimEnd('|');
                    sb.Append(@")\w*\b");

                    rule.Regex = new Regex(sb.ToString());
                    StringBuilderPool.Return(sb);
                    rules.Add(rule);
                }
            }
        }

        private void AvalonLuaEditor_TextEntered(object sender, TextCompositionEventArgs e)
        {
            // Text colon or dot, find the word before it.
            if (e.Text != "." && e.Text != ":")
            {
                return;
            }

            // If the auto complete window is already showing don't show it again.
            if (_completionWindow != null)
            {
                return;
            }

            string word = GetWordBefore(Editor);

            if (word == "mouse" || word == "ui")
            {
                // Open code completion after the user has pressed dot
                _completionWindow = new CompletionWindow(Editor.TextArea);
                var data = _completionWindow.CompletionList.CompletionData;
                LuaCompletion.LoadCompletionData(data, word);
            }

            if (_completionWindow != null)
            {
                _completionWindow.Show();
                _completionWindow.Closed += (sender, args) => _completionWindow = null;
            }
        }

        private void AvalonLuaEditor_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                // Open code completion after the user has pressed dot:
                _completionWindow = new CompletionWindow(Editor.TextArea);
                var data = _completionWindow.CompletionList.CompletionData;
                LuaCompletion.LoadCompletionDataSnippets(data);

                _completionWindow.Show();
                _completionWindow.Closed += (sender, args) => _completionWindow = null;
            }
            else if (e.Key == Key.F5 && !(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            {
                ButtonRunLua_OnClick(sender, e);
            }
            else if (e.Key == Key.F5 && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            {
                ButtonStop_OnClick(sender, e);
            }
            else if (e.Key == Key.K && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                this.CommentOutSelection();
            }
            else if (e.Key == Key.U && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                this.UncommentSelection();
            }
            else if (Keyboard.IsKeyDown(Key.F6))
            {
                ButtonStop_OnClick(sender, e);
            }
        }

        private void AvalonLuaEditor_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _completionWindow.CompletionList.RequestInsertion(e);
                }
            }

            // NOTE: Do not set "e.Handled=true", we still want to insert the character that was typed.
        }

        /// <summary>
        /// Gets the word before the caret.  This seems to work accidentally.  Go through this when
        /// new use cases come up if wonky behavior occurs.
        /// </summary>
        /// <param name="textEditor"></param>
        public static string GetWordBefore(TextEditor textEditor)
        {
            var wordBeforeDot = string.Empty;
            int caretPosition = textEditor.CaretOffset - 2;

            if (caretPosition < 0)
            {
                return wordBeforeDot;
            }

            var lineOffset = textEditor.Document.GetOffset(textEditor.Document.GetLocation(caretPosition));
            string text = textEditor.Document.GetText(lineOffset, 1);

            while (true)
            {
                if (text == null && text.CompareTo(' ') > 0)
                {
                    break;
                }

                if (Regex.IsMatch(text, @".*[^A-Za-z\. ]"))
                {
                    break;
                }

                if (text != "." && text != ":" && text != " ")
                {
                    wordBeforeDot = text + wordBeforeDot;
                }

                if (text == " ")
                {
                    break;
                }

                if (caretPosition == 0)
                {
                    break;
                }

                lineOffset = textEditor.Document.GetOffset(textEditor.Document.GetLocation(--caretPosition));

                text = textEditor.Document.GetText(lineOffset, 1);
            }

            return wordBeforeDot;
        }

        /// <summary>
        /// Comments out any selected code.
        /// </summary>
        private void CommentOutSelection()
        {
            if (this.Editor.SelectionLength <= 1)
            {
                return;
            }

            var sb = StringBuilderPool.Take();
            var lines = this.Editor.SelectedText.Split('\n');

            foreach (var line in lines)
            {
                string codeLine = line.TrimEnd();

                if (codeLine.StartsWith("--"))
                {
                    sb.AppendLine(codeLine);
                }
                else
                {
                    sb.Append("--").Append(codeLine).Append("\r\n");
                }
            }

            this.Editor.SelectedText = sb.ToString().TrimEnd('\r', '\n');

            StringBuilderPool.Return(sb);
        }

        /// <summary>
        /// Uncomment any selected code.
        /// </summary>
        private void UncommentSelection()
        {
            if (this.Editor.SelectionLength <= 1)
            {
                return;
            }

            var sb = StringBuilderPool.Take();
            var lines = this.Editor.SelectedText.Split('\n');

            foreach (var line in lines)
            {
                string codeLine = line.TrimEnd();

                if (codeLine.StartsWith("--"))
                {
                    sb.AppendLine(codeLine.Substring(2));
                }
                else
                {
                    sb.Append(codeLine).Append("\r\n");
                }
            }

            this.Editor.SelectedText = sb.ToString().TrimEnd('\r', '\n');

            StringBuilderPool.Return(sb);
        }

        private void ButtonRecord_OnClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.PlayButtonEnabled = false;
            this.ViewModel.PlayButtonBrush = Brushes.Gray;

            this.ViewModel.RecordButtonEnabled = false;
            this.ViewModel.RecordButtonBrush = Brushes.Gray;

            this.ViewModel.StopButtonEnabled = true;
            this.ViewModel.StopButtonBrush = UIBrushes.RedBrush;

            this.ViewModel.StatusBarForegroundBrush = UIBrushes.WhiteBrush;
            this.ViewModel.StatusBarBackgroundBrush = UIBrushes.RedBrush;

            this.MouseEvents.Clear();
            _recorderStopwatch.Restart();
        }

        /// <summary>
        /// When a full key press occurs.
        /// </summary>
        /// <param name="key"></param>
        private void KeyHookOnKeyUp(VirtualKeys key)
        {
            // If we're not recording, ditch out.
            if (!this._recorderStopwatch.IsRunning)
            {
                return;
            }

            var k = KeyInterop.KeyFromVirtualKey((int)key);
            KeyboardHelper.KeyToChar(k, out var ks);

            if (ks.Printable)
            {
                //ui.SendKeys("")
                Console.AppendText(ks.Character.ToString());
            }
        }

        private void MouseHookOnMouseMove(MouseHookStruct mouse)
        {
            // So we're using the mouse hook to only get the info when the mouse moves
            // but also, it's wrong with DPI stuff.. so we'll then just make the call (knowing
            // the coordinates are different) to GetCursorPos which will correctly return
            // the values.
            Win32.GetCursorPos(out System.Drawing.Point p);

            this.ViewModel.X = p.X;
            this.ViewModel.Y = p.Y;

            // If we're not recording, ditch out.
            if (!this._recorderStopwatch.IsRunning)
            {
                return;
            }

            var e = new MouseEvent
            {
                X = p.X,
                Y = p.Y,
                TimeSpan = new TimeSpan(0, 0, 0, 0, (int)this._recorderStopwatch.ElapsedMilliseconds)
            };

            this.MouseEvents.Add(e);
        }

        private void MouseHookOnLeftButtonUp(MouseHookStruct mouse)
        {
            // First, check for just an individual control click even if not recording, if
            // found put the script commands in for it then get out.
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                // Make sure this always goes on a new line.
                if (Editor.Text.Length > 0 && Editor.Text[^1] != '\n')
                {
                    Editor.AppendText("\r\n");
                }

                Win32.GetCursorPos(out System.Drawing.Point p);
                Editor.AppendText($"mouse.SetPosition({p.X}, {p.Y})\r\n");
                Editor.AppendText("mouse.LeftClick()\r\n");
                Editor.AppendText($"ui.Sleep({AppSettings.ControlClickMilliseconds})\r\n");
                Editor.ScrollToEnd();
                return;
            }

            // If we're not recording, ditch out.
            if (!this._recorderStopwatch.IsRunning)
            {
                return;
            }

            var e = new MouseEvent
            {
                EventType = MouseEventType.LeftUp,
                TimeSpan = new TimeSpan(0, 0, 0, 0, (int)this._recorderStopwatch.ElapsedMilliseconds)
            };

            this.MouseEvents.Add(e);
        }

        private void MouseHookOnLeftButtonDown(MouseHookStruct mouse)
        {
            // If we're not recording, ditch out.
            if (!this._recorderStopwatch.IsRunning)
            {
                return;
            }

            var e = new MouseEvent
            {
                EventType = MouseEventType.LeftDown,
                TimeSpan = new TimeSpan(0, 0, 0, 0, (int)this._recorderStopwatch.ElapsedMilliseconds)
            };

            this.MouseEvents.Add(e);
        }

        private void MouseHookOnRightButtonDown(MouseHookStruct mouse)
        {
            // If we're not recording, ditch out.
            if (!this._recorderStopwatch.IsRunning)
            {
                return;
            }

            var e = new MouseEvent
            {
                EventType = MouseEventType.RightDown,
                TimeSpan = new TimeSpan(0, 0, 0, 0, (int)this._recorderStopwatch.ElapsedMilliseconds)
            };

            this.MouseEvents.Add(e);
        }

        private void MouseHookOnRightButtonUp(MouseHookStruct mouse)
        {
            // First, check for just an individual control click even if not recording, if
            // found put the script commands in for it then get out.
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                // Make sure this always goes on a new line.
                if (Editor.Text.Length > 0 && Editor.Text[^1] != '\n')
                {
                    Editor.AppendText("\r\n");
                }

                Win32.GetCursorPos(out System.Drawing.Point p);
                Editor.AppendText($"mouse.SetPosition({p.X}, {p.Y})\r\n");
                Editor.AppendText("mouse.RightClick()\r\n");
                Editor.AppendText($"ui.Sleep({AppSettings.ControlClickMilliseconds})\r\n");
                Editor.ScrollToEnd();
                return;
            }

            // If we're not recording, ditch out.
            if (!this._recorderStopwatch.IsRunning)
            {
                return;
            }

            var e = new MouseEvent
            {
                EventType = MouseEventType.RightUp,
                TimeSpan = new TimeSpan(0, 0, 0, 0, (int)this._recorderStopwatch.ElapsedMilliseconds)
            };

            this.MouseEvents.Add(e);
        }

        private void MouseHookOnMiddleButtonUp(MouseHookStruct mouse)
        {
            // If we're not recording, ditch out.
            if (!this._recorderStopwatch.IsRunning)
            {
                return;
            }

            var e = new MouseEvent
            {
                EventType = MouseEventType.MiddleUp,
                TimeSpan = new TimeSpan(0, 0, 0, 0, (int)this._recorderStopwatch.ElapsedMilliseconds)
            };

            this.MouseEvents.Add(e);
        }

        private void MouseHookOnMiddleButtonDown(MouseHookStruct mouse)
        {
            // If we're not recording, ditch out.
            if (!this._recorderStopwatch.IsRunning)
            {
                return;
            }

            var e = new MouseEvent
            {
                EventType = MouseEventType.MiddleDown,
                TimeSpan = new TimeSpan(0, 0, 0, 0, (int)this._recorderStopwatch.ElapsedMilliseconds)
            };

            this.MouseEvents.Add(e);
        }

        private async void ButtonOpen_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Lua files (*.lua)|*.lua|Text Files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Open Lua Script"
            };

            if (!string.IsNullOrWhiteSpace(AppSettings.LastSaveDirectory) && Directory.Exists(AppSettings.LastSaveDirectory))
            {
                dialog.InitialDirectory = AppSettings.LastSaveDirectory;
            }

            try
            {
                if (dialog.ShowDialog() == true)
                {
                    Editor.Text = await File.ReadAllTextAsync(dialog.FileName);
                    this.AppSettings.LastSaveDirectory = Path.GetDirectoryName(dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private async void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Lua files (*.lua)|*.lua|Text Files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Save Lua Script"
            };

            if (!string.IsNullOrWhiteSpace(AppSettings.LastSaveDirectory) && Directory.Exists(AppSettings.LastSaveDirectory))
            {
                dialog.InitialDirectory = AppSettings.LastSaveDirectory;
            }

            try
            {
                if (dialog.ShowDialog() == true)
                {
                    await File.WriteAllTextAsync(dialog.FileName, Editor.Text);
                    this.AppSettings.LastSaveDirectory = Path.GetDirectoryName(dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
