/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using Argus.Memory;
using MouseAutomation.Common;
using System.Windows;

namespace MouseAutomation.Pages
{
    public partial class SettingsPage
    {
        public AppSettings AppSettings { get; set; }

        public SettingsPage()
        {
            this.InitializeComponent();
            this.AppSettings = AppServices.GetRequiredService<AppSettings>();
            this.DataContext = this.AppSettings;
            AppServices.AddSingleton(this);
        }

        private void PollingInterval_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var page = AppServices.GetRequiredService<LuaEditorPage>();
        }
    }
}
