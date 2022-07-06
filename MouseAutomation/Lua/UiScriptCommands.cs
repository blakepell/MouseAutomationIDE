﻿/*
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
using System.Threading.Tasks;
using System.Windows;

namespace MouseAutomation.Lua
{
    /// <summary>
    /// UI Script Commands
    /// </summary>
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
                    var editor = AppServices.GetRequiredService<LuaEditorPage>();
                    return editor.StatusText;
                }

                string text = "";

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var editor = AppServices.GetRequiredService<LuaEditorPage>();
                    text = editor.StatusText;
                });

                return text;
            }
            set
            {
                // If it has access directly set it.
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    var editor = AppServices.GetRequiredService<LuaEditorPage>();
                    editor.StatusText = value;
                    return;
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var editor = AppServices.GetRequiredService<LuaEditorPage>();
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
        /// Pauses the lua script for a designated amount of milliseconds.  This should work with both
        /// sync and not sync Lua calls.
        /// </summary>
        /// <param name="milliseconds"></param>
        [Description("Pauses a Lua script for the designated amount of milliseconds.")]
        public void Pause(int milliseconds)
        {
            Task.Delay(milliseconds).Wait();
        }
    }
}