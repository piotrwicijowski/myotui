using System.Collections.Generic;
using Terminal.Gui;
using System.Linq;

namespace myotui.Models
{
    public class VBoxBuffer : IBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<ILayoutContent> Content {get; set;}
    }
}