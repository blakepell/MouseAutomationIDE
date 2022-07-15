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
    [StructLayout(LayoutKind.Sequential)]
    public struct MouseHookStruct
    {
        public PointStruct pt;
        public uint mouseData;
        public LowLevelMouseEvent flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
}