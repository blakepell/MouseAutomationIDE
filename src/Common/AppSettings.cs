/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

namespace LuaAutomation.Common
{
    /// <summary>
    /// AppSettings that are persisted to storage and reloaded on application launch.
    /// </summary>
    public class AppSettings : DependencyObject
    {
        /// <summary>
        /// The text that's in the editor when the app closes.
        /// </summary>
        public string? AutoSaveText { get; set; }

        /// <summary>
        /// The last directory a file was open or saved in.
        /// </summary>
        public string? LastSaveDirectory { get; set; }

        [JsonIgnore]
        public static readonly DependencyProperty AutoSaveOnExitProperty = DependencyProperty.Register(
            nameof(AutoSaveOnExit), typeof(bool), typeof(AppSettings), new PropertyMetadata(true));

        public bool AutoSaveOnExit
        {
            get => (bool)GetValue(AutoSaveOnExitProperty);
            set => SetValue(AutoSaveOnExitProperty, value);
        }

        [JsonIgnore]
        public static readonly DependencyProperty ConvertTabsToSpacesProperty = DependencyProperty.Register(
            nameof(ConvertTabsToSpaces), typeof(bool), typeof(AppSettings), new PropertyMetadata(true));

        public bool ConvertTabsToSpaces
        {
            get => (bool)GetValue(ConvertTabsToSpacesProperty);
            set => SetValue(ConvertTabsToSpacesProperty, value);
        }

        [JsonIgnore]
        public static readonly DependencyProperty ShowConsoleOnRunProperty = DependencyProperty.Register(
            nameof(ShowConsoleOnRun), typeof(bool), typeof(AppSettings), new PropertyMetadata(true));

        public bool ShowConsoleOnRun
        {
            get => (bool)GetValue(ShowConsoleOnRunProperty);
            set => SetValue(ShowConsoleOnRunProperty, value);
        }

        [JsonIgnore]
        public static readonly DependencyProperty ShowConsoleOnStartupProperty = DependencyProperty.Register(
            nameof(ShowConsoleOnStartup), typeof(bool), typeof(AppSettings), new PropertyMetadata(false));

        public bool ShowConsoleOnStartup
        {
            get => (bool)GetValue(ShowConsoleOnStartupProperty);
            set => SetValue(ShowConsoleOnStartupProperty, value);
        }

        [JsonIgnore]
        public static readonly DependencyProperty ShowTimestampOnConsoleProperty = DependencyProperty.Register(
            nameof(ShowTimestampOnConsole), typeof(bool), typeof(AppSettings), new PropertyMetadata(false));

        public bool ShowTimestampOnConsole
        {
            get => (bool)GetValue(ShowTimestampOnConsoleProperty);
            set => SetValue(ShowTimestampOnConsoleProperty, value);
        }

        [JsonIgnore]
        public static readonly DependencyProperty ClearConsoleOnRunProperty = DependencyProperty.Register(
            nameof(ClearConsoleOnRun), typeof(bool), typeof(AppSettings), new PropertyMetadata(true));

        public bool ClearConsoleOnRun
        {
            get => (bool)GetValue(ClearConsoleOnRunProperty);
            set => SetValue(ClearConsoleOnRunProperty, value);
        }

        [JsonIgnore]
        public static readonly DependencyProperty ControlClickMillisecondsProperty = DependencyProperty.Register(
            nameof(ControlClickMilliseconds), typeof(int), typeof(AppSettings), new PropertyMetadata(500));

        public int ControlClickMilliseconds
        {
            get => (int)GetValue(ControlClickMillisecondsProperty);
            set => SetValue(ControlClickMillisecondsProperty, value);
        }
    }
}
