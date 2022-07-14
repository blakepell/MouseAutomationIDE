/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace LuaAutomation.Lua
{
    /// <summary>
    /// Autocomplete data for Lua.
    /// </summary>
    public class LuaCompletionData : ICompletionData
    {
        public LuaCompletionData(string text)
        {
            this.Text = text;
        }

        public LuaCompletionData(string text, string description)
        {
            this.Text = text;
            this.Description = description;
        }

        public LuaCompletionData(string text, string description, string contentPrefix)
        {
            this.Text = text;
            this.Description = description;
            this.ContentPrefix = contentPrefix;
        }

        public LuaCompletionData(string text, string description, string contentPrefix, double priority)
        {
            this.Text = text;
            this.Description = description;
            this.ContentPrefix = contentPrefix;
            this.Priority = priority;
        }

        public System.Windows.Media.ImageSource Image { get; set; } = null;

        /// <summary>
        /// Actual text to insert.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// A prefix that displays before the Content display.
        /// </summary>
        public string ContentPrefix { get; set; } = "";

        /// <summary>
        /// Use this property if you want to show a fancy UIElement in the list that displays.
        /// </summary>
        public object Content => $"{this.ContentPrefix}{this.Text}";

        public object Description { get; set; }

        public double Priority { get; set; } = 1.0;

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            if (this.Text == "For Loop")
            {
                var sb = Argus.Memory.StringBuilderPool.Take();
                sb.AppendLine("for i = 1, 10 do");
                sb.AppendLine("   -- i");
                sb.AppendLine("end");

                textArea.Document.Replace(completionSegment, sb.ToString());
                Argus.Memory.StringBuilderPool.Return(sb);
            }
            else if (this.Text == "For Loop Pairs")
            {
                var sb = Argus.Memory.StringBuilderPool.Take();
                sb.AppendLine("for k, v in pairs(arr) do");
                sb.AppendLine("   --(k, v[1], v[2], v[3])");
                sb.AppendLine("end");

                textArea.Document.Replace(completionSegment, sb.ToString());
                Argus.Memory.StringBuilderPool.Return(sb);
            }
            else if (this.Text == "While Loop")
            {
                var sb = Argus.Memory.StringBuilderPool.Take();

                sb.Append(@"local condition = true
while(condition)
do
    -- Pause for 10 seconds
    ui.PauseAsync(10000)
end");

                textArea.Document.Replace(completionSegment, sb.ToString());
                Argus.Memory.StringBuilderPool.Return(sb);
            }
            else
            {
                textArea.Document.Replace(completionSegment, this.Text);
            }
        }
    }
}