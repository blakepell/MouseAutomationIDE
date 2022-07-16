/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using System.Drawing.Design;
using LuaAutomation.Pages;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Tray;

namespace LuaAutomation
{
    public partial class MainWindow : INavigationWindow
    {
        /// <summary>
        /// The view model for the <see cref="LuaEditorPage"/>.
        /// </summary>
        public MainWindowViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // Register our shared objects with the DI container.
            this.ViewModel = new();
            AppServices.AddSingleton(this);
            AppServices.AddSingleton(this.ViewModel);

            this.DataContext = this.ViewModel;

            // Get our app settings.
            var appSettings = AppServices.GetRequiredService<AppSettings>();

            // TODO: Have this bind directly
            // Reset the minimize to tray setting.
            TitleBar.MinimizeToTray = appSettings.MinimizeToTray;
        }

        /// <summary>
        /// When the icon in the system tray is left clicked on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_OnLeftClick(INotifyIcon sender, RoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        #region INavigationWindow methods

        public Frame GetFrame() => RootFrame;

        public INavigation GetNavigation() => RootNavigation;

        public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService) => RootNavigation.PageService = pageService;

        public void ShowWindow() => Show();

        public void CloseWindow() => Close();

        #endregion INavigationWindow methods

        private void MenuItemStopScript_OnClick(object sender, RoutedEventArgs e)
        {
            var page = AppServices.GetRequiredService<LuaEditorPage>();
            page.Stop();
        }

        private void MenuItemOpen_OnClick(object sender, RoutedEventArgs e)
        {
            var page = AppServices.GetRequiredService<LuaEditorPage>();
            page.Open();
        }

        private void MenuItemExit_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO: DI
            var ui = new UIScriptCommands();
            ui.Exit();
        }
    }
}
