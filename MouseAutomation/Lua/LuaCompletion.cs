/*
 * Mouse Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using ICSharpCode.AvalonEdit.CodeCompletion;

namespace MouseAutomation.Lua
{
    public static class LuaCompletion
    {
        /// <summary>
        /// A static list of completion data we can construct and format once.  We are going
        /// to consolidate overloads.
        /// </summary>
        private static Dictionary<string, ICompletionData>? _luaCompletionData;

        private static Dictionary<string, ICompletionData>? _uiCompletionData;

        /// <summary>
        /// Loads the completion data based on the pattern.
        /// </summary>
        public static void LoadCompletionData(IList<ICompletionData> data, string pattern)
        {
            if (pattern == "mouse")
            {
                if (_luaCompletionData == null)
                {
                    // Initialize the Lua completion data once.
                    _luaCompletionData = new Dictionary<string, ICompletionData>();

                    var t = typeof(MouseScriptCommands);

                    // This should get all of our methods but exclude ones that are defined on
                    // object like ToString, GetHashCode, Equals, etc.
                    foreach (var method in t.GetMethods().Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object)).OrderBy(m => m.Name))
                    {
                        if (_luaCompletionData.ContainsKey(method.Name))
                        {
                            // It exists, therefore this is an overload.
                            ConstructMembers(method, (LuaCompletionData) _luaCompletionData[method.Name]);
                        }
                        else
                        {
                            var lcd = new LuaCompletionData(method.Name, "");
                            _luaCompletionData.Add(method.Name, lcd);
                            ConstructMembers(method, lcd);
                            data.Add(lcd);
                        }
                    }

                    foreach (var prop in t.GetProperties().Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object)).OrderBy(m => m.Name))
                    {
                        if (_luaCompletionData.ContainsKey(prop.Name))
                        {
                            // It exists, therefore this is an overload.
                            ConstructProperties(prop, (LuaCompletionData)_luaCompletionData[prop.Name]);
                        }
                        else
                        {
                            var lcd = new LuaCompletionData(prop.Name, "");
                            _luaCompletionData.Add(prop.Name, lcd);
                            ConstructProperties(prop, lcd);
                            data.Add(lcd);
                        }
                    }

                }
                else
                {
                    foreach (var item in _luaCompletionData)
                    {
                        data.Add(item.Value);
                    }
                }
            }
            else if (pattern == "ui")
            {
                if (_uiCompletionData == null)
                {
                    // Initialize the Lua completion data once.
                    _uiCompletionData = new Dictionary<string, ICompletionData>();

                    var t = typeof(UIScriptCommands);

                    // This should get all of our methods but exclude ones that are defined on
                    // object like ToString, GetHashCode, Equals, etc.
                    foreach (var method in t.GetMethods().Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object)).OrderBy(m => m.Name))
                    {
                        if (_uiCompletionData.ContainsKey(method.Name))
                        {
                            // It exists, therefore this is an overload.
                            ConstructMembers(method, (LuaCompletionData)_uiCompletionData[method.Name]);
                        }
                        else
                        {
                            var lcd = new LuaCompletionData(method.Name, "");
                            _uiCompletionData.Add(method.Name, lcd);
                            ConstructMembers(method, lcd);
                            data.Add(lcd);
                        }
                    }

                    foreach (var prop in t.GetProperties().Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object)).OrderBy(m => m.Name))
                    {
                        if (_uiCompletionData.ContainsKey(prop.Name))
                        {
                            // It exists, therefore this is an overload.
                            ConstructProperties(prop, (LuaCompletionData)_uiCompletionData[prop.Name]);
                        }
                        else
                        {
                            var lcd = new LuaCompletionData(prop.Name, "");
                            _uiCompletionData.Add(prop.Name, lcd);
                            ConstructProperties(prop, lcd);
                            data.Add(lcd);
                        }
                    }

                }
                else
                {
                    foreach (var item in _uiCompletionData)
                    {
                        data.Add(item.Value);
                    }
                }
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
