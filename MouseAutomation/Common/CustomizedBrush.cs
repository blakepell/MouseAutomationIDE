﻿/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;

// ReSharper disable UnusedMember.Global

namespace MouseAutomation.Common
{
    /// <summary>
    /// A <see cref="HighlightingBrush"/> that can be used to colorize text in AvalonEdit.
    /// </summary>
    internal sealed class CustomizedBrush : HighlightingBrush
    {
        private readonly SolidColorBrush _brush;

        public CustomizedBrush(Color color)
        {
            _brush = CreateFrozenBrush(color);
        }

        public CustomizedBrush(System.Drawing.Color c)
        {
            var c2 = Color.FromArgb(c.A, c.R, c.G, c.B);
            _brush = CreateFrozenBrush(c2);
        }

        public override Brush GetBrush(ITextRunConstructionContext context)
        {
            return _brush;
        }

        public override string ToString()
        {
            return _brush.ToString();
        }

        private static SolidColorBrush CreateFrozenBrush(Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }
    }
}