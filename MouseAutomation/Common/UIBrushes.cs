/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using System.Windows.Media;

namespace MouseAutomation.Common
{
    /// <summary>
    /// Static frozen brushes used on the UI.
    /// </summary>
    public class UIBrushes
    {
        public static SolidColorBrush RedBrush;

        public static SolidColorBrush GreenBrush;

        public static void Init()
        {
            RedBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#C04048");
            RedBrush?.Freeze();

            GreenBrush = Brushes.Green;
            GreenBrush.Freeze();
        }
    }
}
