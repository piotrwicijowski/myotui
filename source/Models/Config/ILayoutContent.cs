using System.Collections.Generic;
using Terminal.Gui;

namespace myotui.Models.Config
{
    public interface ILayoutContent
    {
        public string Name {get; set;}
        public string Value {get; set;}
        public SizeHint Width {get; set;}
        public SizeHint Height {get; set;}
        public IEnumerable<IParameter> Parameters {get; set;}
    }
}