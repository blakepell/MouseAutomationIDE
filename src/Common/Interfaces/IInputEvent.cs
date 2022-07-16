/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

namespace LuaAutomation.Common.Interfaces
{
    /// <summary>
    /// Interface for input events.
    /// </summary>
    public interface IInputEvent
    {
        public TimeSpan? TimeSpan { get; init; }

        public int DelayMilliseconds { get; set; }
    }
}
