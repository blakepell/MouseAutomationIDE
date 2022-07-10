/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using System.Windows;
using Argus.Memory;
using MouseAutomation.Common;

namespace MouseAutomation.Pages
{
    public partial class LuaSyntaxHelpPage
    {
        public AppSettings AppSettings { get; set; }

        public LuaSyntaxHelpPage()
        {
            this.InitializeComponent();
            this.AppSettings = AppServices.GetRequiredService<AppSettings>();
            this.DataContext = this.AppSettings;
            AppServices.AddSingleton(this);
        }

        private void LuaSyntaxHelpPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            WebBrowser.NavigateToString(@"<html><body style='background: #1E1E1E; color: White; font-family: Consolas;'><h2>Lua Extensions</h2><hr /></body></html>");
        }
    }
}
