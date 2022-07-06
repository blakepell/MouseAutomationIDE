/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using Argus.Extensions;
using Argus.Memory;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using MoonSharp.Interpreter;
using MouseAutomation.Common;
using MouseAutomation.Lua;
using MouseAutomation.Pages;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Diagnostics;

namespace MouseAutomation.Controls
{
    /// <summary>
    /// Interaction logic for AvalonLuaEditor.xaml
    /// </summary>
    public partial class AvalonLuaEditor
    {
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

        private Script Script { get; set; }

        public static readonly DependencyProperty PlayButtonEnabledProperty = DependencyProperty.Register(
            nameof(PlayButtonEnabled), typeof(bool), typeof(AvalonLuaEditor), new PropertyMetadata(true));

        /// <summary>
        /// If a Lua script is currently running.
        /// </summary>
        public bool PlayButtonEnabled
        {
            get => (bool)GetValue(PlayButtonEnabledProperty);
            set => SetValue(PlayButtonEnabledProperty, value);
        }

        public static readonly DependencyProperty PlayButtonBrushProperty = DependencyProperty.Register(
            "PlayButtonBrush", typeof(SolidColorBrush), typeof(AvalonLuaEditor), new PropertyMetadata(Brushes.Green));

        /// <summary>
        /// The color of the play button.
        /// </summary>
        public SolidColorBrush PlayButtonBrush
        {
            get => (SolidColorBrush)GetValue(PlayButtonBrushProperty);
            set => SetValue(PlayButtonBrushProperty, value);
        }

        public AvalonLuaEditor()
        {
            InitializeComponent();
            this.DataContext = this;

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

            this.Script = new();
            this.Script.Options.CheckThreadAccess = false;
            this.Script.Globals.Set("mouse", UserData.Create(mouseCommands));
            this.Script.Globals.Set("ui", UserData.Create(uiCommands));

            Script.WarmUp();

            AppServices.AddSingleton(this);
        }

        private void AvalonLuaEditor_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.SetupLuaEditor();
            
            Editor.TextArea.TextEntering += AvalonLuaEditor_TextEntering;
            Editor.TextArea.TextEntered += AvalonLuaEditor_TextEntered;
        }

        /// <summary>
        /// Runs a Lua script in the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonRunLua_OnClick(object sender, RoutedEventArgs e)
        {
            var luaPage = AppServices.GetRequiredService<LuaEditorPage>();
            _executionControlToken = new();

            this.PlayButtonEnabled = false;
            this.PlayButtonBrush = Brushes.Gray;

            var sw = new Stopwatch();

            try
            {
                sw.Start();
                _ = await this.Script.DoStringAsync(_executionControlToken, this.Editor.Text);
                sw.Stop();
                luaPage.StatusText = $"Completed => Runtime {sw.ElapsedMilliseconds / 1000}s";
            }
            catch (Exception ex)
            {
                sw.Stop();
                luaPage.StatusText = $"Error: {ex.Message} => Runtime: {sw.ElapsedMilliseconds / 1000}s";
            }
            finally
            {
                this.PlayButtonEnabled = true;
                this.PlayButtonBrush = Brushes.Green;
            }
        }

        /// <summary>
        /// Stops a Lua script that's running.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStop_OnClick(object sender, RoutedEventArgs e)
        {
            _executionControlToken.Terminate();
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
                            Foreground = new CustomizedBrush((Color) ColorConverter.ConvertFromString(("#DCDCAA")))
                        }
                    };

                    // Construct our custom highlighting rule via reflection.
                    var t = typeof(MouseScriptCommands);
                    var ui = typeof(UIScriptCommands);

                    var sb = new StringBuilder();
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
                    rules.Add(rule);
                }
            }
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
                LuaCompletion.LoadCompletionDatasnippets(data);

                _completionWindow.Show();
                _completionWindow.Closed += (sender, args) => _completionWindow = null;

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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCommentOutSelection(object sender, RoutedEventArgs e)
        {
            if (this.Editor.SelectionLength <= 1)
            {
                return;
            }

            var sb = Argus.Memory.StringBuilderPool.Take();
            string code = this.Editor.SelectedText;
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

            this.Editor.SelectedText = sb.ToString();

            Argus.Memory.StringBuilderPool.Return(sb);
        }

        /// <summary>
        /// Uncomments any selected code.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonUncommentSelection(object sender, RoutedEventArgs e)
        {
            if (this.Editor.SelectionLength <= 1)
            {
                return;
            }

            var sb = Argus.Memory.StringBuilderPool.Take();
            string code = this.Editor.SelectedText;
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

            this.Editor.SelectedText = sb.ToString();

            Argus.Memory.StringBuilderPool.Return(sb);
        }
    }
}