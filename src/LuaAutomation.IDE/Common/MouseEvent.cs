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
    public enum MouseEventType
    {
        MouseMove = 0,
        LeftDown = 1,
        LeftUp = 2,
        RightDown = 3,
        RightUp = 4,
        MiddleDown = 5,
        MiddleUp = 6,
        ScrollUp = 7,
        ScrollDown = 8
    }

    public class MouseEvent : IInputEvent
    {
        public TimeSpan? TimeSpan { get; init; }

        public int DelayMilliseconds { get; set; } = 0;

        public MouseEventType EventType = MouseEventType.MouseMove;

        public int X { get; init; }

        public int Y { get; init; }
    }
}
