/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using LuaAutomation.Common;

namespace LuaAutomation.Pages
{
    public partial class SettingsPage
    {
        private AppSettings AppSettings { get; set; }

        public SettingsPage()
        {
            this.InitializeComponent();
            this.AppSettings = AppServices.GetRequiredService<AppSettings>();
            this.DataContext = this.AppSettings;
            AppServices.AddSingleton(this);
        }

        private void CheckBoxConvertTabsToSpace_OnChecked(object sender, RoutedEventArgs e)
        {
            var page = AppServices.GetRequiredService<LuaEditorPage>();
            page.Editor.Options.ConvertTabsToSpaces = CheckBoxConvertTabsToSpaces.IsChecked ?? false;
        }
    }
}
