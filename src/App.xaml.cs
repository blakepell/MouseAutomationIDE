/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using LuaAutomation.Common;
using Mouse = Argus.Windows.Mouse;

namespace LuaAutomation
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public partial class App
    {
        private const string APP_FOLDER = "MouseAutomation";

        public const string APP_TITLE = "Lua Automation IDE";

        /// <summary>
        /// MouseHook class for use with recording mouse macros.
        /// </summary>
        internal static MouseHook MouseHook = new();

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

            try
            {
                MouseHook.Install();
            }
            catch (Exception ex)
            {
                // TODO: Logging
            }
        }

        private async void App_OnExit(object sender, ExitEventArgs e)
        {
            // Make sure to show the mouse when we exit in case a script hid it.
            Mouse.MouseShow();

            var appSettings = AppServices.GetRequiredService<AppSettings>() ?? new AppSettings();

            // Save the settings.
            if (!appSettings.AutoSaveOnExit)
            {
                appSettings.AutoSaveText = null;
            }

            var file = new Argus.IO.JsonFileService(Environment.SpecialFolder.LocalApplicationData, APP_FOLDER);
            await file.SaveAsync(appSettings, "AppSettings.json");

            try
            {
                MouseHook.Uninstall();
            }
            catch (Exception exception)
            {
                // TODO: Logging
            }
        }
    }
}
