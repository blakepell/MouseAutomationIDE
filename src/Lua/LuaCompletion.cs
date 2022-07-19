/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using ICSharpCode.AvalonEdit.CodeCompletion;

namespace LuaAutomation.Lua
{
    public static class LuaCompletion
    {
        /// <summary>
        /// Static list of the auto complete data.
        /// </summary>
        private static Dictionary<string, List<LuaCompletionData>> CompletionData { get; set; }

        /// <summary>
        /// Constructs the static list of completion data based off of any class that has
        /// a <see cref="LuaClassAttribute"/> on it.
        /// </summary>
        static LuaCompletion()
        {
            CompletionData = new();

            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsDefined(typeof(LuaClassAttribute)));

            foreach (var t in types)
            {
                var attr = t.GetCustomAttributes(false)
                                                .OfType<LuaClassAttribute>()
                                                .SingleOrDefault();

                if (attr == null)
                {
                    continue;
                }

                var entries = new List<LuaCompletionData>();
                CompletionData.Add(attr.Name, entries);

                // This should get all of our methods but exclude ones that are defined on
                // object like ToString, GetHashCode, Equals, etc.
                foreach (var method in t.GetMethods().Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object)).OrderBy(m => m.Name))
                {
                    var entry = entries.FirstOrDefault(x => x.Text == method.Name);

                    if (entry != null)
                    {
                        // It exists, therefore this is an overload.
                        ConstructMembers(method, entry);
                    }
                    else
                    {
                        var lcd = new LuaCompletionData(method.Name, "");
                        ConstructMembers(method, lcd);
                        entries.Add(lcd);
                    }
                }

                foreach (var prop in t.GetProperties().Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object)).OrderBy(m => m.Name))
                {
                    var entry = entries.FirstOrDefault(x => x.Text == prop.Name);

                    if (entry != null)
                    {
                        // It exists, therefore this is an overload.
                        ConstructProperties(prop, entry);
                    }
                    else
                    {
                        var lcd = new LuaCompletionData(prop.Name, "");
                        ConstructProperties(prop, lcd);
                        entries.Add(lcd);
                    }
                }

            }
        }

        /// <summary>
        /// Loads the completion data based on the pattern.
        /// </summary>
        public static void LoadCompletionData(IList<ICompletionData> data, string pattern)
        {
            if (!CompletionData.ContainsKey(pattern))
            {
                return;
            }

            var entries = CompletionData[pattern];

            foreach (var entry in entries)
            {
                data.Add(entry);
            }
        }

        /// <summary>
        /// Constructs the data about a method and it's overloads.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="lcd"></param>
        private static void ConstructMembers(MethodInfo method, LuaCompletionData lcd)
        {
            var sb = Argus.Memory.StringBuilderPool.Take();

            // Whether to start with just the method name (for the first one) or append
            // the parameters as an overload.
            if (string.IsNullOrWhiteSpace((string)lcd.Description))
            {
                sb.AppendLine("Function");
                sb.AppendLine($"{method.Name} => Returns {method.ReturnType}");
                sb.Replace("System.Void", "nothing");

                if (method.CustomAttributes.Any())
                {
                    var attr = method.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name.Contains("DescriptionAttribute"));

                    if (attr?.ConstructorArguments.Count > 0)
                    {
                        sb.AppendLine(attr.ConstructorArguments[0].ToString().Trim('"'));
                    }
                }

            }
            else
            {
                sb.AppendLine((string)lcd.Description);
            }

            var parameterDescriptions = string.Join
                (", ", method.GetParameters()
                     .Select(x => $"{x.ParameterType} {x.Name}")
                     .ToArray());

            // The call and parameters
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

            // Trim the final line break off.
            sb.TrimEnd('\r', '\n');

            lcd.Description = sb.ToString();

            Argus.Memory.StringBuilderPool.Return(sb);
        }

        /// <summary>
        /// Constructs the data about a property.
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="lcd"></param>
        private static void ConstructProperties(PropertyInfo prop, LuaCompletionData lcd)
        {
            var sb = Argus.Memory.StringBuilderPool.Take();

            // Whether to start with just the method name (for the first one) or append
            // the parameters as an overload.
            if (string.IsNullOrWhiteSpace((string)lcd.Description))
            {
                sb.AppendLine("Property");
                sb.AppendLine($"{prop.Name} => Gets or sets {prop.PropertyType}");
                sb.Replace("System.Void", "nothing");

                if (prop.CustomAttributes.Any())
                {
                    var attr = prop.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name.Contains("DescriptionAttribute"));

                    if (attr?.ConstructorArguments.Count > 0)
                    {
                        sb.AppendLine(attr.ConstructorArguments[0].ToString().Trim('"'));
                    }
                }
            }
            else
            {
                sb.AppendLine((string)lcd.Description);
            }

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

            // Trim the final line break off.
            sb.TrimEnd('\r', '\n');

            lcd.Description = sb.ToString();

            Argus.Memory.StringBuilderPool.Return(sb);
        }

        public static void LoadCompletionDataSnippets(IList<ICompletionData> data)
        {
            data.Add(new LuaCompletionData("For Loop", "A snippet to show how to do a basic for loop.", ""));
            data.Add(new LuaCompletionData("For Loop Pairs", "A snippet to show iterate over pairs.", ""));
            data.Add(new LuaCompletionData("While Loop", "A snippet to to show how to do a while loop with pausing.", ""));
        }
    }
}
