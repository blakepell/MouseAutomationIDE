/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using System.Linq;
using System.Text;
using System.Windows;
using Argus.Memory;
using MouseAutomation.Common;
using MouseAutomation.Lua;

namespace MouseAutomation.Pages
{
    public partial class LuaSyntaxHelpPage
    {
        public AppSettings AppSettings { get; set; }

        public LuaSyntaxHelpPage()
        {
            this.InitializeComponent();
            this.AppSettings = AppServices.GetRequiredService<AppSettings>();
            this.DataContext = this.AppSettings;
            AppServices.AddSingleton(this);
        }

        private void LuaSyntaxHelpPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append("<html><body style='background: #272F33; color: White; font-family: Segoe UI; padding-top: 10px;'>");
            sb.Append("<h2>Lua Syntax Documentation</h2><hr />");

            sb.Append("<h3>Mouse Methods</h3>");
            sb.Append("<table style='width: 100%;'>");

            var t = typeof(MouseScriptCommands);

            // This should get all of our methods but exclude ones that are defined on
            // object like ToString, GetHashCode, Equals, etc.
            foreach (var method in t.GetMethods().Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object)).OrderBy(m => m.Name))
            {
                sb.Append("<tr>");
                sb.Append("<td style='vertical-align: top;'>mouse.");

                var parameterDescriptions = string.Join
                    (", ", method.GetParameters()
                         .Select(x => $"{x.ParameterType} {x.Name}")
                         .ToArray());

                sb.AppendLine($"{method.Name}({parameterDescriptions})");

                // Cleanup the parameters to make them easier to read.
                sb.Replace("System.String", "string")
                    .Replace("System.Int32", "number")
                    .Replace("System.DataTime", "datetime")
                    .Replace("System.Boolean", "boolean")
                    .Replace("System.Collections.Generic", "")
                    .Replace("System.Object", "object")
                    .Replace("Avalon.Common.Models.", "")
                    .Replace("MahApps.Metro.IconPacks.", "");

                // Remove any double line breaks if they exist.
                sb.Replace("\r\n\r\n", "\r\n");

                sb.Append("</td>");
                sb.Append("<td style='vertical-align: top;'>");
                sb.AppendLine($"Returns {method.ReturnType}<br />");
                sb.Replace("System.Void", "nothing");

                if (method.CustomAttributes.Any())
                {
                    var attr = method.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name.Contains("DescriptionAttribute"));

                    if (attr?.ConstructorArguments.Count > 0)
                    {
                        sb.AppendLine(attr.ConstructorArguments[0].ToString().Trim('"'));
                    }
                }

                sb.Append("</td></tr>");
            }
            sb.Append("</table>");

            sb.Append("<h3>Mouse Properties</h3>");
            sb.Append("<table style='width: 100%;'>");

            foreach (var prop in t.GetProperties().Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object)).OrderBy(m => m.Name))
            {
                sb.AppendLine($"<td>mouse.{prop.Name}</td><td>Gets or sets {prop.PropertyType}");
                sb.Replace("System.Void", "nothing");

                if (prop.CustomAttributes.Any())
                {
                    var attr = prop.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name.Contains("DescriptionAttribute"));

                    if (attr?.ConstructorArguments.Count > 0)
                    {
                        sb.AppendLine("<br >").Append(attr.ConstructorArguments[0].ToString().Trim('"'));
                    }
                }

                sb.Append("</td></tr>");

                // Cleanup the parameters to make them easier to read.
                sb.Replace("System.String", "string")
                    .Replace("System.Int32", "number")
                    .Replace("System.DataTime", "datetime")
                    .Replace("System.Boolean", "boolean")
                    .Replace("System.Collections.Generic", "")
                    .Replace("System.Object", "object")
                    .Replace("Avalon.Common.Models.", "")
                    .Replace("MahApps.Metro.IconPacks.", "");

                // Remove any double line breaks if they exist.
                sb.Replace("\r\n\r\n", "\r\n");
            }

            sb.Append("</table>");

            sb.Append("</body></html>");

            WebBrowser.NavigateToString(sb.ToString());
        }
    }
}
