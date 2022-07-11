/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;

namespace MouseAutomation.Common
{
    /// <summary>
    /// Static frozen brushes used on the UI.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class UIBrushes
    {
        public static SolidColorBrush RedBrush;

        public static SolidColorBrush GreenBrush;

        public static SolidColorBrush GrayBrush;

        public static SolidColorBrush LightBlueBrush;

        public static SolidColorBrush WhiteBrush;

        public static SolidColorBrush OrangeBrush;

        public static SolidColorBrush BlackBrush;

        public static void Init()
        {
            RedBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#C04048");
            RedBrush?.Freeze();

            GreenBrush = Brushes.Green;
            GreenBrush.Freeze();

            GrayBrush = Brushes.Gray;
            GrayBrush.Freeze();

            LightBlueBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#006CBF");
            LightBlueBrush?.Freeze();

            WhiteBrush = Brushes.White;
            WhiteBrush.Freeze();

            OrangeBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF8C00");
            OrangeBrush?.Freeze();

            BlackBrush = Brushes.Black;
            BlackBrush.Freeze();
        }
    }
}
