using System.Collections.Generic;
using Terminal.Gui;
using System.Linq;

namespace myotui.Models
{
    public class HBoxBuffer : ILayoutBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<IBinding> Bindings {get; set;}
        public IEnumerable<ILayoutContent> Windows {get; set;}
    }
}