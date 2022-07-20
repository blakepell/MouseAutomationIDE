/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using LuaAutomation.Pages;
using Rect = LuaAutomation.Common.Windows.Rect;

namespace LuaAutomation.Lua
{
    /// <summary>
    /// UI Script Commands
    /// </summary>
    [LuaClass("ui")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class UIScriptCommands
    {
        /// <summary>
        /// Status text at the bottom of the editor window.
        /// </summary>
        [Description("Status text at the bottom of the editor window.")]
        public string StatusText
        {
            get
            {
                // If it has access directly return it.
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    var editor = AppServices.GetRequiredService<LuaEditorViewModel>();
                    return editor.StatusText;
                }

                string text = "";

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var editor = AppServices.GetRequiredService<LuaEditorViewModel>();
                    text = editor.StatusText;
                });

                return text;
            }
            set
            {
                // If it has access directly set it.
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    var editor = AppServices.GetRequiredService<LuaEditorViewModel>();
                    editor.StatusText = value;
                    return;
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var editor = AppServices.GetRequiredService<LuaEditorViewModel>();
                    editor.StatusText = value;
                });
            }
        }

        /// <summary>
        /// The window title.
        /// </summary>
        [Description("The window title.")]
        public string Title
        {
            get
            {
                // If it has access directly return it.
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    var editor = AppServices.GetRequiredService<MainWindowViewModel>();
                    return editor.Title;
                }

                string text = "";

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var editor = AppServices.GetRequiredService<MainWindowViewModel>();
                    text = editor.Title;
                });

                return text;
            }
            set
            {
                // If it has access directly set it.
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    var editor = AppServices.GetRequiredService<MainWindowViewModel>();
                    editor.Title = value;
                    return;
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var editor = AppServices.GetRequiredService<MainWindowViewModel>();
                    editor.Title = value;
                });
            }
        }

        [Description("Shows a message box with the specified text.")]
        public void MsgBox(string text)
        {
            // If it doesn't have access then execute the same function on the UI thread, otherwise just run it.
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => MsgBox(text)));
                return;
            }

            MessageBox.Show(text);
        }

        [Description("Shutdown down the IDE.")]
        public void Exit()
        {
            // If it doesn't have access then execute the same function on the UI thread, otherwise just run it.
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(this.Exit));
                return;
            }

            Application.Current.Shutdown();
        }

        [Description("Minimizes the IDE.")]
        public void Minimize()
        {
            // If it doesn't have access then execute the same function on the UI thread, otherwise just run it.
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(this.Minimize));
                return;
            }

            var win = AppServices.GetRequiredService<MainWindow>();
            win.WindowState = WindowState.Minimized;
        }

        [Description("Maximizes the IDE.")]
        public void Maximize()
        {
            // If it doesn't have access then execute the same function on the UI thread, otherwise just run it.
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(this.Maximize));
                return;
            }

            var win = AppServices.GetRequiredService<MainWindow>();
            win.WindowState = WindowState.Maximized;
        }

        [Description("Maximizes the IDE.")]
        public void Activate()
        {
            // If it doesn't have access then execute the same function on the UI thread, otherwise just run it.
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(this.Activate));
                return;
            }

            var win = AppServices.GetRequiredService<MainWindow>();
            win.WindowState = WindowState.Normal;
            win.Activate();
        }

        /// <summary>
        /// Hides the IDE window.
        /// </summary>
        [Description("Hides the IDE window.")]
        public void HideWindow()
        {
            // If it doesn't have access then execute the same function on the UI thread, otherwise just run it.
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(this.HideWindow));
                return;
            }

            var win = AppServices.GetRequiredService<MainWindow>();
            win.Hide();
        }

        /// <summary>
        /// Shows the IDE window.
        /// </summary>
        [Description("Shows the IDE window.")]
        public void ShowWindow()
        {
            // If it doesn't have access then execute the same function on the UI thread, otherwise just run it.
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(this.ShowWindow));
                return;
            }

            var win = AppServices.GetRequiredService<MainWindow>();
            win.Show();
        }

        [Description("Calls Thread.Sleep for the specified number of milliseconds (prefer this for long pauses).  This is CPU efficient but sometimes inaccurate (1ms pauses can sometimes take up to 10).")]
        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        private Stopwatch _sw = new();

        [Description("Calls an accurate but CPU intensive pause for the specified amount of milliseconds (prefer this for pauses between 1-10ms).")]
        public void Pause(int milliseconds)
        {
            _sw.Restart();

            while (_sw.ElapsedMilliseconds < milliseconds)
            {

            }

            _sw.Stop();
        }

        [Description("Simulates keystrokes.")]
        public void SendKeys(string keys)
        {
            System.Windows.Forms.SendKeys.SendWait(keys);
            System.Windows.Forms.SendKeys.Flush();
        }

        [Description("Writes a log entry to the console.")]
        public void Log(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            // If it doesn't have access then execute the same function on the UI thread, otherwise just run it.
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => this.Log(text));
                return;
            }

            var editor = AppServices.GetRequiredService<LuaEditorPage>();
            var appSettings = AppServices.GetRequiredService<AppSettings>();

            if (appSettings.ShowTimestampOnConsole)
            {
                editor.Console.AppendText($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()} :: ");
            }

            editor.Console.AppendText(text);
            editor.Console.AppendText(Environment.NewLine);
            editor.Console.ScrollToEnd();
        }

        [Description("Clears all of the text in the console.")]
        public void ClearConsole()
        {
            // If it doesn't have access then execute the same function on the UI thread, otherwise just run it.
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(this.ClearConsole);
                return;
            }

            var editor = AppServices.GetRequiredService<LuaEditorPage>();
            editor.Console.Text = "";
        }

        [Description("Moves the cursor to the specified line in the code editor.")]
        public void EditorGotoLine(int line)
        {
            // If it doesn't have access then execute the same function on the UI thread, otherwise just run it.
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => this.EditorGotoLine(line));
                return;
            }

            var editor = AppServices.GetRequiredService<LuaEditorPage>();
            editor.Editor.ScrollToLine(line);
            editor.Editor.TextArea.Caret.Line = line;
            editor.Editor.TextArea.Caret.Column = 0;
        }

        [Description("Moves the cursor to the specified line in the console.")]
        public void ConsoleGotoLine(int line)
        {
            // If it doesn't have access then execute the same function on the UI thread, otherwise just run it.
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => this.ConsoleGotoLine(line));
                return;
            }

            var editor = AppServices.GetRequiredService<LuaEditorPage>();
            editor.Console.ScrollToLine(line);
        }

        /// <summary>
        /// TODO: Move into Win32 class.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [Description("Returns a Rect for the specified process if it's found.")]
        public Rect GetWindowPosition(string processName)
        {
            var rect = new Rect();
            var teamsProc = Process.GetProcessesByName(processName).FirstOrDefault();

            if (teamsProc == null)
            {
                return rect;
            }

            _ = GetWindowRect(teamsProc.MainWindowHandle, ref rect);
            return rect;
        }

        [Description("Sets the focus to the specified process.")]
        public void SetWindowFocus(string processName)
        {
            var proc = Process.GetProcessesByName(processName).FirstOrDefault();

            if (proc == null)
            {
                return;
            }

            _ = Argus.Windows.Window.SetForegroundWindow(proc.MainWindowHandle);
        }

        [Description("Sets the focus to the specified process.")]
        public void SetWindowFocus(int processId)
        {
            var proc = Process.GetProcessById(processId);
            _ = Argus.Windows.Window.SetForegroundWindow(proc.MainWindowHandle);
        }
    }
}