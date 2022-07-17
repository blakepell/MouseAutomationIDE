/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

namespace LuaAutomation.Pages
{
    public partial class ExamplesPage
    {
        public List<string> LuaExamples => new()
        {
            "while loop example", 
            "for loop example"
        };

        public ExamplesPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
    }
}
