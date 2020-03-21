using System.Collections.Generic;
using System.Linq;
using ConsoleFramework;
using ConsoleFramework.Controls;

namespace myotui.Models
{
    public class App : IApp
    {
        public string Name {get; set;}

        public IEnumerable<IModeDefinition> Modes {get; set;}
        public IEnumerable<IBuffer> Buffers {get; set;}

        private Panel GetRootContent()
        {
            var rootBuffer = Buffers.FirstOrDefault(x => x.Name == "root");
            return rootBuffer.BuildLayout();
        }

        public Window BuildWindow()
        {
            var rootContent = GetRootContent();
            var window = new Window
            {
                Name = Name,
                Title = Name,
                Content = rootContent,
                // MinWidth = int.MaxValue,
                // MinHeight = int.MaxValue

            };
            return window;
        }
    }
}