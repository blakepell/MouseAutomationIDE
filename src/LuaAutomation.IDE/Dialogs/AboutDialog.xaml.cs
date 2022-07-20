/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

namespace LuaAutomation.Dialogs
{
    public partial class AboutDialog
    {
        public static readonly DependencyProperty VersionProperty = DependencyProperty.Register(
            nameof(Version), typeof(string), typeof(AboutDialog), new PropertyMetadata(default(string)));

        public string Version
        {
            get => (string)GetValue(VersionProperty);
            set => SetValue(VersionProperty, value);
        }

        public AboutDialog()
        {
            InitializeComponent();
            this.DataContext = this;

            string version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
            string bit = Environment.Is64BitProcess ? "64-bit" : "32-bit";

            this.Version = $"{version} {bit}";
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
