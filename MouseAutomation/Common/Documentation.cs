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
    /// Creates documentation from reflection on an object and via custom attributes.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class Documentation
    {
        public string BackgroundColor { get; set; } = "#272F33";

        public string ForegroundColor { get; set; } = "White";

        private List<string> _docs = new();

        public void AddHtmlDoc(string header, Type t)
        {
            var sb = StringBuilderPool.Take();

            this.GenerateMethods(header, t, sb);
            this.GenerateProperties(header, t, sb);

            _docs.Add(sb.ToString());
            StringBuilderPool.Return(sb);
        }

        private void GenerateProperties(string header, Type t, StringBuilder sb)
        {
            sb.Append("<h3>").Append(header).Append(" Properties</h3>");
            sb.Append("<table style='width: 100%;'>");

            foreach (var prop in t.GetProperties().Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object)).OrderBy(m => m.Name))
            {
                sb.Append("<tr style=\"background: #191E21\">");

                sb.AppendLine($"<td class='cell'><span class=\"class\">{header.ToLower()}</span>.{prop.Name}</td><td class='cell'>Gets or sets {prop.PropertyType}");
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

                Cleanup(sb);
            }

            sb.Append("</table>");
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
                sb.Append($"<td class='cell'><span class=\"class\">{header.ToLower()}</span>.");

                var parameterDescriptions = string.Join
                (", ", method.GetParameters()
                    .Select(x => $"<span class=\"type\">{x.ParameterType}</span> {x.Name}")
                    .ToArray());

                sb.AppendLine($"<span class=\"method\">{method.Name}</span>({parameterDescriptions})");

                // Cleanup the parameters to make them easier to read.
                Cleanup(sb);

                sb.Append("</td>");
                sb.Append("<td class='cell'>");
                sb.AppendLine($"Returns <span class='type'>{method.ReturnType}</span><br />");
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
    .cell {
        padding: 10px;
        vertical-align: top;
        width: 50%;
    }
</style>
");
            sb.Append($"<body style='background: {this.BackgroundColor}; color: {this.ForegroundColor}; font-family: Segoe UI; padding-top: 10px;'>");
            sb.Append("<h2>Lua Syntax Extensions Documentation</h2><hr />");

            foreach (string doc in this._docs)
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
