/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

namespace LuaAutomation.Common
{
    public class KeyEvent : IInputEvent
    {
        public TimeSpan? TimeSpan { get; init; }

        public int DelayMilliseconds { get; set; } = 0;

        public string? SendKeysValue { get; set; }
    }
}