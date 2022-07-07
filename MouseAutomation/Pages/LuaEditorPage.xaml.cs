/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using System;
using MouseAutomation.Common;
using System.Windows;
using System.Windows.Threading;
using Argus.Memory;
using Argus.Windows;
using MouseAutomation.Controls;

namespace MouseAutomation.Pages
{
    public partial class LuaEditorPage
    {
        public AppSettings AppSettings { get; set; }

        /// <summary>
        /// The dispatch timer responsible for polling the mouse coordinates.
        /// </summary>
        public DispatcherTimer Timer { get; set; }

        public static readonly DependencyProperty XProperty = DependencyProperty.Register(
            nameof(X), typeof(int), typeof(LuaEditorPage), new PropertyMetadata(default(int)));

        /// <summary>
        /// The current Y coordinate the mouse is at.
        /// </summary>
        public int X
        {
            get => (int)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public static readonly DependencyProperty YProperty = DependencyProperty.Register(
            nameof(Y), typeof(int), typeof(LuaEditorPage), new PropertyMetadata(default(int)));

        /// <summary>
        /// The current Y coordinate the mouse is at.
        /// </summary>
        public int Y
        {
            get => (int)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public static readonly DependencyProperty StatusTextProperty = DependencyProperty.Register(
            nameof(StatusText), typeof(string), typeof(LuaEditorPage), new PropertyMetadata(default(string)));

        /// <summary>
        /// Status text at the bottom of the code window.
        /// </summary>
        public string StatusText
        {
            get => (string)GetValue(StatusTextProperty);
            set => SetValue(StatusTextProperty, value);
        }

        public LuaEditorPage()
        {
            this.InitializeComponent();
            this.AppSettings = AppServices.GetRequiredService<AppSettings>();
            this.DataContext = this;
            this.LuaEditor.Editor.Text = this.AppSettings.AutoSaveText;

            // Setup the mouse polling event.
            this.Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(this.AppSettings.PollingInterval)
            };

            this.Timer.Tick += this.Timer_Tick;
            this.Timer.Start();

            AppServices.AddSingleton(this);

            this.StatusText = "Idle";
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            int x = Mouse.X();
            int y = Mouse.Y();

            // If there is no change ditch out early.
            if (this.X == x || this.Y == y)
            {
                return;
            }

            this.X = Mouse.X();
            this.Y = Mouse.Y();

            //var editor = AppServices.GetRequiredService<AvalonLuaEditor>();
            //editor.Editor.AppendText($"mouse.SetPosition({this.X}, {this.Y})\r\n");
            //editor.Editor.AppendText($"ui.Pause({this.AppSettings.PollingInterval})\r\n");
        }

        private void LuaEditorPage_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                this.AppSettings.AutoSaveText = this.LuaEditor.Editor.Text;
            }
        }
    }
}
