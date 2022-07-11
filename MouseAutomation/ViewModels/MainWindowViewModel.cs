/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using System.Windows;

namespace MouseAutomation.ViewModels
{
    public class MainWindowViewModel : DependencyObject
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title), typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(App.APP_TITLE));

        /// <summary>
        /// The title of the IDE window.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
    }
}