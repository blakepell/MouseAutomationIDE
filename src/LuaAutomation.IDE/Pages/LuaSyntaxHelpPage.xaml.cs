/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

namespace LuaAutomation.Pages
{
    public partial class LuaSyntaxHelpPage
    {
        private AppSettings AppSettings { get; set; }

        public LuaSyntaxHelpPage()
        {
            this.InitializeComponent();
            this.AppSettings = AppServices.GetRequiredService<AppSettings>();
            this.DataContext = this.AppSettings;
            AppServices.AddSingleton(this);
        }

        private void LuaSyntaxHelpPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            var docs = new Documentation();

            var mouse = typeof(MouseScriptCommands);
            docs.AddHtmlDoc("Mouse", mouse);

            var ui = typeof(UIScriptCommands);
            docs.AddHtmlDoc("UI", ui);

            var file = typeof(FileScriptCommands);
            docs.AddHtmlDoc("File", file);

            WebBrowser.NavigateToString(docs.Generate());
        }
    }
}
