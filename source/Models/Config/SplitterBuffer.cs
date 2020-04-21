using System.Collections.Generic;
namespace myotui.Models.Config
{
    public class SplitterBuffer : IBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<IBinding> Bindings {get; set;}
        public IEnumerable<IParameter> Parameters {get; set;}
        public bool Closable {get; set;} = false;
        public bool Focusable {get; set;} = false;
        public IValueContent Content {get; set;}
    }
}