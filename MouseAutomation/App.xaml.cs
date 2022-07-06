/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using Argus.Memory;
using Argus.Windows;
using MouseAutomation.Common;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace MouseAutomation
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public partial class App
    {
        private const string APP_FOLDER = "MouseAutomation";

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            try
            {
                // Reading in the settings async caused it to be created on a different
                // thread than the UI and a "Must create DependencySource on same Thread as the DependencyObject."
                // exception was occurring.
                var file = new Argus.IO.JsonFileService(Environment.SpecialFolder.LocalApplicationData, APP_FOLDER);
                var appSettings = file.Read<AppSettings>("AppSettings.json") ?? new AppSettings();
                AppServices.AddSingleton(appSettings);
            }
            catch
            {
                AppServices.AddSingleton(new AppSettings());
            }
        }

        private async void App_OnExit(object sender, ExitEventArgs e)
        {
            // Make sure to show the mouse when we exit in case a script hid it.
            Mouse.MouseShow();

            // Save the settings.
            var file = new Argus.IO.JsonFileService(Environment.SpecialFolder.LocalApplicationData, APP_FOLDER);
            var appSettings = AppServices.GetRequiredService<AppSettings>() ?? new AppSettings();
            await file.SaveAsync(appSettings, "AppSettings.json");
        }
    }
}
