/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using System.Text.Json.Serialization;
using System.Windows;

namespace MouseAutomation.Common
{
    /// <summary>
    /// AppSettings that are persisted to storage and reloaded on application launch.
    /// </summary>
    public class AppSettings : DependencyObject
    {
        public string? AutoSaveText { get; set; }

        [JsonIgnore]
        public static readonly DependencyProperty PollingIntervalProperty = DependencyProperty.Register(
            nameof(PollingInterval), typeof(int), typeof(AppSettings), new PropertyMetadata(100));

        public int PollingInterval
        {
            get => (int)GetValue(PollingIntervalProperty);
            set => SetValue(PollingIntervalProperty, value);
        }

        [JsonIgnore]
        public static readonly DependencyProperty AutoSaveOnExitProperty = DependencyProperty.Register(
            nameof(AutoSaveOnExit), typeof(bool), typeof(AppSettings), new PropertyMetadata(true));

        public bool AutoSaveOnExit
        {
            get => (bool)GetValue(AutoSaveOnExitProperty);
            set => SetValue(AutoSaveOnExitProperty, value);
        }
    }
}
