/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

namespace MouseAutomation.Pages
{
    public partial class HotKeysPage
    {
        public HotKeysPage()
        {
            this.InitializeComponent();
        }

        private void HotKeysPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            var sb = StringBuilderPool.Take();

            sb.Append("<html>");
            sb.Append(@"
<style>
    .code {
        font-family: Consolas;
    }
    .class {
        color: #D2A0DF;
    }
    .type {
        color: #569CD6;
    }
    .method  {
        color: #DCDCAA;
    }
    .cell {
        padding: 10px;
        vertical-align: top;
        width: 50%;
    }
</style>
");
            sb.Append($"<body style='background: #272F33; color: White; font-family: Segoe UI; padding-top: 10px;'>");
            sb.Append("<h2>Hot Keys</h2><hr />");

            sb.Append("<table style='width: 100%;'>");
            sb.AppendLine($"<tr style=\"background: #191E21\"><td class='cell'><span class=\"class\">F1</span></td><td class='cell'>Show Snippets</td></tr>");
            sb.AppendLine($"<tr style=\"background: #191E21\"><td class='cell'><span class=\"class\">F5</span></td><td class='cell'>Runs Program</td></tr>");
            sb.AppendLine($"<tr style=\"background: #191E21\"><td class='cell'><span class=\"class\">Shift+F5</span></td><td class='cell'>Stops Running Program</td></tr>");
            sb.AppendLine($"<tr style=\"background: #191E21\"><td class='cell'><span class=\"class\">Ctrl+K</span></td><td class='cell'>Comments out selection</td></tr>");
            sb.AppendLine($"<tr style=\"background: #191E21\"><td class='cell'><span class=\"class\">Ctrl+U</span></td><td class='cell'>Uncomment selection</td></tr>");

            sb.Append("</table>");

            sb.Append("</body></html>");

            WebBrowser.NavigateToString(sb.ToString());

            StringBuilderPool.Return(sb);
        }
    }
}
