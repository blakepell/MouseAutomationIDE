/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using MouseAutomation.Common;
using System.Windows;
using System.Windows.Media;

namespace MouseAutomation.ViewModels
{
    public class LuaEditorViewModel : DependencyObject
    {
        public static readonly DependencyProperty XProperty = DependencyProperty.Register(
            nameof(X), typeof(int), typeof(LuaEditorViewModel), new PropertyMetadata(default(int)));

        /// <summary>
        /// The current Y coordinate the mouse is at.
        /// </summary>
        public int X
        {
            get => (int)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public static readonly DependencyProperty YProperty = DependencyProperty.Register(
            nameof(Y), typeof(int), typeof(LuaEditorViewModel), new PropertyMetadata(default(int)));

        /// <summary>
        /// The current Y coordinate the mouse is at.
        /// </summary>
        public int Y
        {
            get => (int)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public static readonly DependencyProperty StatusTextProperty = DependencyProperty.Register(
            nameof(StatusText), typeof(string), typeof(LuaEditorViewModel), new PropertyMetadata(default(string)));

        /// <summary>
        /// Status text at the bottom of the code window.
        /// </summary>
        public string StatusText
        {
            get => (string)GetValue(StatusTextProperty);
            set => SetValue(StatusTextProperty, value);
        }

        public static readonly DependencyProperty PlayButtonEnabledProperty = DependencyProperty.Register(
            nameof(PlayButtonEnabled), typeof(bool), typeof(LuaEditorViewModel), new PropertyMetadata(true));

        /// <summary>
        /// If a Lua script is currently running.
        /// </summary>
        public bool PlayButtonEnabled
        {
            get => (bool)GetValue(PlayButtonEnabledProperty);
            set => SetValue(PlayButtonEnabledProperty, value);
        }

        public static readonly DependencyProperty PlayButtonBrushProperty = DependencyProperty.Register(
            "PlayButtonBrush", typeof(SolidColorBrush), typeof(LuaEditorViewModel), new PropertyMetadata(UIBrushes.GreenBrush));

        /// <summary>
        /// The color of the play button.
        /// </summary>
        public SolidColorBrush PlayButtonBrush
        {
            get => (SolidColorBrush)GetValue(PlayButtonBrushProperty);
            set => SetValue(PlayButtonBrushProperty, value);
        }

        public static readonly DependencyProperty RecordButtonEnabledProperty = DependencyProperty.Register(
            nameof(RecordButtonEnabled), typeof(bool), typeof(LuaEditorViewModel), new PropertyMetadata(true));

        public bool RecordButtonEnabled
        {
            get => (bool)GetValue(RecordButtonEnabledProperty);
            set => SetValue(RecordButtonEnabledProperty, value);
        }

        public static readonly DependencyProperty RecordButtonBrushProperty = DependencyProperty.Register(
            "RecordButtonBrush", typeof(SolidColorBrush), typeof(LuaEditorViewModel), new PropertyMetadata(UIBrushes.RedBrush));

        /// <summary>
        /// The color of the Record button.
        /// </summary>
        public SolidColorBrush RecordButtonBrush
        {
            get => (SolidColorBrush)GetValue(RecordButtonBrushProperty);
            set => SetValue(RecordButtonBrushProperty, value);
        }

        public static readonly DependencyProperty StopButtonEnabledProperty = DependencyProperty.Register(
            nameof(StopButtonEnabled), typeof(bool), typeof(LuaEditorViewModel), new PropertyMetadata(true));

        public bool StopButtonEnabled
        {
            get => (bool)GetValue(StopButtonEnabledProperty);
            set => SetValue(StopButtonEnabledProperty, value);
        }

        public static readonly DependencyProperty StopButtonBrushProperty = DependencyProperty.Register(
            "StopButtonBrush", typeof(SolidColorBrush), typeof(LuaEditorViewModel), new PropertyMetadata(UIBrushes.RedBrush));

        /// <summary>
        /// The color of the Stop button.
        /// </summary>
        public SolidColorBrush StopButtonBrush
        {
            get => (SolidColorBrush)GetValue(StopButtonBrushProperty);
            set => SetValue(StopButtonBrushProperty, value);
        }

        public static readonly DependencyProperty StatusBarBackgroundBrushProperty = DependencyProperty.Register(
            "StatusBarBackgroundBrush", typeof(SolidColorBrush), typeof(LuaEditorViewModel), new PropertyMetadata(UIBrushes.LightBlueBrush));

        /// <summary>
        /// The background color of the status bar.
        /// </summary>
        public SolidColorBrush StatusBarBackgroundBrush
        {
            get => (SolidColorBrush)GetValue(StatusBarBackgroundBrushProperty);
            set => SetValue(StatusBarBackgroundBrushProperty, value);
        }

        public static readonly DependencyProperty StatusBarForegroundBrushProperty = DependencyProperty.Register(
            "StatusBarForegroundBrush", typeof(SolidColorBrush), typeof(LuaEditorViewModel), new PropertyMetadata(UIBrushes.WhiteBrush));

        /// <summary>
        /// The foreground color of the status bar.
        /// </summary>
        public SolidColorBrush StatusBarForegroundBrush
        {
            get => (SolidColorBrush)GetValue(StatusBarForegroundBrushProperty);
            set => SetValue(StatusBarForegroundBrushProperty, value);
        }

    }
}
