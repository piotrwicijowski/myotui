using System.Collections.Generic;
using ConsoleFramework;
using ConsoleFramework.Controls;

namespace myotui.Models
{
    public interface IBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<IContent> Content {get; set;}
        public Panel BuildLayout()
        {
            var panel = new Panel
            {
                Name = Name
            };
            return panel;
        }
    }
}