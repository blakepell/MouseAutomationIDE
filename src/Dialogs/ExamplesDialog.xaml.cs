/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

using LuaAutomation.Pages;

namespace LuaAutomation.Dialogs
{
    public partial class ExamplesDialog
    {
        public class ExampleData
        {
            public ExampleData(string name, string resourceFileName)
            {
                this.Name = name;
                this.ResourceFileName = resourceFileName;
            }

            public string Name { get; set; }

            public string ResourceFileName { get; set; }
        }

        public List<ExampleData> LuaExamples => new()
        {
            new ExampleData("Click on a running application", "s.lua"),
            new ExampleData("Find a window by process name", ""),
            new ExampleData("For Loop Examples", "ExampleForLoop.lua"),
            new ExampleData("While Loop Examples","ExampleWhileLoop.lua"),
            new ExampleData("File Reading and Writing",""),
            new ExampleData("Updating UI Elements",""),
            new ExampleData("Mouse Automation", "")
        };

        public ExamplesDialog()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonAddProgramToEditor_OnClick(object sender, RoutedEventArgs e)
        {
            var page = AppServices.GetRequiredService<LuaEditorPage>();
            var item = ListLuaExamples.SelectedItem as ExampleData;
            var ui = AppServices.GetRequiredService<UIScriptCommands>();

            if (item == null || string.IsNullOrEmpty(item.ResourceFileName))
            {
                this.Close();
                return;
            }

            var asm = Assembly.GetExecutingAssembly();
            string result = "";

            using (var s = asm.GetManifestResourceStream($"{asm.GetName().Name}.Resources.{item.ResourceFileName}"))
            {
                if (s == null)
                {
                    ui.StatusText = $"Embedded Resource {item.ResourceFileName} was not found.";
                    this.Close();
                    return;
                }

                using (var sr = new StreamReader(s))
                {
                    result = sr.ReadToEnd();
                }
            }

            page.Editor.Text = result;
            this.Close();
        }
    }
}
