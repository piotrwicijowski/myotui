using System.Collections.Generic;
using Terminal.Gui;
using System.Linq;

namespace myotui.Models.Config
{
    public class VBoxBuffer : ILayoutBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<IBinding> Bindings {get; set;}
        public IEnumerable<IParameter> Parameters {get; set;}
        public IEnumerable<ILayoutContent> Windows {get; set;}
        public bool AutoSplitters {get; set;} = true;
        public bool Closable {get; set;} = false;
    }
}