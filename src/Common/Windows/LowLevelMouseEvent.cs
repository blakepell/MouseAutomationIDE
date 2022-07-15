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
    [Flags]
    public enum LowLevelMouseEvent : uint
    {
        None = 0x00,
        Injected = 0x01,
        LowerIntegrityLevelInjected = 0x02
    }
}
