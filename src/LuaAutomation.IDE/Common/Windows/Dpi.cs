/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

namespace LuaAutomation.Common.Windows
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class Dpi
    {
        public static void SetProcessDpiAware()
        {
            _ = NativeMethods.SetProcessDPIAware();
        }

        public static void SetProcessDpiAware(ProcessDpiAwareness pda)
        {
            _ = NativeMethods.SetProcessDpiAwareness(pda);
        }
    }
}