/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using MouseAutomation.Pages;
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
            nameof(StatusText), typeof(string), typeof(LuaEditorPage), new PropertyMetadata(default(string)));

        /// <summary>
        /// Status text at the bottom of the code window.
        /// </summary>
        public string StatusText
        {
            get => (string)GetValue(StatusTextProperty);
            set => SetValue(StatusTextProperty, value);
        }

        public static readonly DependencyProperty PlayButtonEnabledProperty = DependencyProperty.Register(
            nameof(PlayButtonEnabled), typeof(bool), typeof(LuaEditorPage), new PropertyMetadata(true));

        /// <summary>
        /// If a Lua script is currently running.
        /// </summary>
        public bool PlayButtonEnabled
        {
            get => (bool)GetValue(PlayButtonEnabledProperty);
            set => SetValue(PlayButtonEnabledProperty, value);
        }

        public static readonly DependencyProperty PlayButtonBrushProperty = DependencyProperty.Register(
            "PlayButtonBrush", typeof(SolidColorBrush), typeof(LuaEditorPage), new PropertyMetadata(Brushes.Green));

        /// <summary>
        /// The color of the play button.
        /// </summary>
        public SolidColorBrush PlayButtonBrush
        {
            get => (SolidColorBrush)GetValue(PlayButtonBrushProperty);
            set => SetValue(PlayButtonBrushProperty, value);
        }

    }
}
