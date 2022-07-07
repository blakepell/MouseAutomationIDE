/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using System;

namespace MouseAutomation.Common
{
    public enum MouseEventType
    {
        MouseMove = 0,
        LeftDown = 1,
        LeftUp = 2
    }

    public class MouseEvent
    {
        public MouseEventType EventType = MouseEventType.MouseMove;

        public TimeSpan? TimeSpan { get; set; }

        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;

        public int DelayMilliseconds = 0;
    }
}
