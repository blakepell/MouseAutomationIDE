/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using Argus.Memory;
using MouseAutomation.Pages;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MouseAutomation.ViewModels;

namespace MouseAutomation.Lua
{
    /// <summary>
    /// UI Script Commands
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
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

        /// <summary>
        /// Will pause the Lua script for the designated amount of milliseconds.  This is not async
        /// so it will block the Lua (but since Lua is called async the rest of the program continues
        /// to work).  This will be an incredibly useful and powerful command for those crafting Lua scripts.
        /// </summary>
        /// <param name="milliseconds"></param>
        [Description("Pauses a Lua script for the designated amount of milliseconds.")]
        public void Pause(int milliseconds)
        {
            // ReSharper disable once AsyncConverter.AsyncWait
            Task.Delay(milliseconds).Wait();
        }

        /// <summary>
        /// Pauses the lua script for a designated amount of milliseconds.  This should work with both
        /// sync and not sync Lua calls.
        /// </summary>
        /// <param name="milliseconds"></param>
        [Description("Pauses a Lua script for the designated amount of milliseconds.")]
        public void PauseAsync(int milliseconds)
        {
            DispatcherFrame df = new DispatcherFrame();

            new Thread(() =>
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(milliseconds));
                df.Continue = false;

            }).Start();

            Dispatcher.PushFrame(df);
        }

        [Description("Calls Thread.Sleep for the specified number of milliseconds.")]
        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        [Description("Simulates keystrokes.")]
        public void SendKeys(string keys)
        {
            System.Windows.Forms.SendKeys.SendWait(keys);
            System.Windows.Forms.SendKeys.Flush();
        }

    }
}