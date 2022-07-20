/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

namespace LuaAutomation.Lua
{
    /// <summary>
    /// A Lua attribute put on a class that exposes it to Lua.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LuaClassAttribute : Attribute
    {
        public LuaClassAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}