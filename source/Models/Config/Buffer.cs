using System.Collections.Generic;
using Terminal.Gui;

namespace myotui.Models.Config
{
    public class Buffer : IBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<IBinding> Bindings {get; set;}
        public IEnumerable<IParameter> Parameters {get; set;}
        public virtual bool Closable {get; set;}
        public virtual bool Focusable {get; set;}
        public IValueContent Content {get; set;}
        public virtual bool DefaultFocus {get; set;} = false;
    }
}