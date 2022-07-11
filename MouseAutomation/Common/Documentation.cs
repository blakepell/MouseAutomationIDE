/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using Argus.Memory;
using MouseAutomation.Lua;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MouseAutomation.Common
{
    /// <summary>
    /// Creates documentation from reflection on an object and via custom attributes.
    /// </summary>
    public class Documentation
    {
        /// <summary>
        /// #, #171717
        /// </summary>
        public string BackgroundColor { get; set; } = "#272F33";

        public string ForegroundColor { get; set; } = "White";

        public List<string> Docs = new();

        public void AddHtmlDoc(string header, Type t)
        {
            var sb = StringBuilderPool.Take();

            GenerateMethods(header, t, sb);

            Docs.Add(sb.ToString());
            StringBuilderPool.Return(sb);
        }

        /// <summary>
        /// Generates the methods for a Type.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="t"></param>
        /// <param name="sb"></param>
        private void GenerateMethods(string header, Type t, StringBuilder sb)
        {
            sb.Append("<h3>").Append(header).Append(" Methods</h3>");
            sb.Append("<table style='width: 100%;'>");

            // This should get all of our methods but exclude ones that are defined on
            // object like ToString, GetHashCode, Equals, etc.
            foreach (var method in t.GetMethods().Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object)).OrderBy(m => m.Name))
            {
                sb.Append("<tr style=\"background: #191E21\">");
                sb.Append($"<td style='vertical-align: top; width: 50%;'><span class=\"class\">{header.ToLower()}</span>.");

                var parameterDescriptions = string.Join
                (", ", method.GetParameters()
                    .Select(x => $"<span class=\"type\">{x.ParameterType}</span> {x.Name}")
                    .ToArray());

                sb.AppendLine($"<span class=\"method\">{method.Name}</span>({parameterDescriptions})");

                // Cleanup the parameters to make them easier to read.
                Cleanup(sb);

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
        }

        public string Generate()
        {
            var sb = new StringBuilder();
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
</style>
");
            sb.Append($"<body style='background: {this.BackgroundColor}; color: {this.ForegroundColor}; font-family: Segoe UI; padding-top: 10px;'>");
            sb.Append("<h2>Lua Syntax Documentation</h2><hr />");

            foreach (string doc in this.Docs)
            {
                sb.Append(doc);
            }

            sb.Append("</body></html>");

            return sb.ToString();
        }

        /// <summary>
        /// Cleanup the reflected string to make it easier to read.
        /// </summary>
        /// <param name="sb"></param>
        private void Cleanup(StringBuilder sb)
        {
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

    }
}
