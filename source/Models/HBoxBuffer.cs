using System.Collections.Generic;
using Terminal.Gui;
using System.Linq;

namespace myotui.Models
{
    public class HBoxBuffer : IBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<IContent> Content {get; set;}
    }
}