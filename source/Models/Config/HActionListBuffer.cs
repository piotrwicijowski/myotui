using System.Collections.Generic;
namespace myotui.Models.Config
{
    public class HActionListBuffer : IBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<IBinding> Bindings {get; set;}
        public IEnumerable<IParameter> Parameters {get; set;}
        public IValueContent Content {get; set;}
        public bool Closable {get; set;} = false;
        public bool Focusable {get; set;} = true;
        public bool HasSearch {get; set;} = true;
    }
}