using System.Collections.Generic;
using ConsoleFramework;
using ConsoleFramework.Controls;
namespace myotui.Models
{
    public class HBoxBuffer : IBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<IContent> Content {get; set;}
        public Panel BuildLayout()
        {
            var panel = new Panel();
            panel.Name = Name;
            panel.Orientation = Orientation.Vertical;
            panel.HorizontalAlignment =  HorizontalAlignment.Stretch;
            panel.VerticalAlignment = VerticalAlignment.Stretch;
            return panel;
        }
    }
}