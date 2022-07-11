/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

namespace MouseAutomation.Common
{
    /// <summary>
    /// Static frozen brushes used on the UI.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class UIBrushes
    {
        static UIBrushes()
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

            OrangeBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46800");
            OrangeBrush?.Freeze();

            BlackBrush = Brushes.Black;
            BlackBrush.Freeze();
        }

        public static SolidColorBrush RedBrush;

        public static SolidColorBrush GreenBrush;

        public static SolidColorBrush GrayBrush;

        public static SolidColorBrush LightBlueBrush;

        public static SolidColorBrush WhiteBrush;

        public static SolidColorBrush OrangeBrush;

        public static SolidColorBrush BlackBrush;
    }
}
