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

            public string Name { get; }

            public string ResourceFileName { get; }
        }

        public List<ExampleData> LuaExamples => new()
        {
            new ExampleData("Click on a running application", "ExampleClickOnARunningApplication.lua"),
            new ExampleData("Find a window position by process name", "ExampleFindAWindowByProcessName.lua"),
            new ExampleData("For Loop Examples", "ExampleForLoop.lua"),
            new ExampleData("While Loop Examples","ExampleWhileLoop.lua"),
            new ExampleData("File Reading and Writing","ExampleFileReadingAndWriting.lua"),
            new ExampleData("Updating UI Elements","ExampleUpdateUiElements.lua"),
            new ExampleData("Mouse Automation", "ExampleMouseAutomation.lua")
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

            using (var s = asm.GetManifestResourceStream($"LuaAutomation.Resources.{item.ResourceFileName}"))
            {
                if (s == null)
                {
                    ui.StatusText = $"[WARNING] Embedded Resource {item.ResourceFileName} was not found.";
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

            // Change the focus to the Lua editor after inserting the code.
            var win = AppServices.GetRequiredService<MainWindow>();
            win.RootNavigation.Navigate("LuaEditorPage");
        }
    }
}
