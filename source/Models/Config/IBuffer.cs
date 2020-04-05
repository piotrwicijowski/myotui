using System.Collections.Generic;
using Terminal.Gui;

namespace myotui.Models.Config
{
    public interface IBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<IBinding> Bindings {get; set;}
        public IEnumerable<IParameter> Parameters {get; set;}
        public bool Closable {get; set;}
        public bool Focusable {get; set;}
    }
}