/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using LuaAutomation.Pages;
using Rect = LuaAutomation.Common.Windows.Rect;

namespace LuaAutomation.Lua
{
    /// <summary>
    /// File Script Commands
    /// </summary>
    [LuaClass("file")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class FileScriptCommands
    {
        [Description("Appends the specified text to a file.")]
        public void Append(string filename, string text)
        {
            System.IO.File.AppendAllText(filename, text);
        }

        [Description("Writes the specified text to a file, overwriting any contents in the file.")]
        public void Write(string filename, string text)
        {
            System.IO.File.WriteAllText(filename, text);
        }

        [Description("Reads the entire contents of a file into a string.")]
        public string Read(string filename)
        {
            return System.IO.File.ReadAllText(filename);
        }

        [Description("Reads all lines and returns the contents as string array.")]
        public string[] ReadLines(string filename)
        {
            return System.IO.File.ReadAllLines(filename);
        }

        [Description("Deletes the specified file.")]
        public void Delete(string filename)
        {
            System.IO.File.Delete(filename);
        }

        [Description("Returns whether a file exists or not.")]
        public bool Exists(string filename)
        {
            return System.IO.File.Exists(filename);
        }

        [Description("Copies one file to another (both old and new files exist aftewards).")]
        public void Copy(string sourceFilename, string destinationFilename)
        {
            System.IO.File.Copy(sourceFilename, destinationFilename);
        }

        [Description("Moves one file to another (the old file no longer exists afterwards).")]
        public void Move(string sourceFilename, string destinationFilename)
        {
            System.IO.File.Copy(sourceFilename, destinationFilename);
        }
    }
}