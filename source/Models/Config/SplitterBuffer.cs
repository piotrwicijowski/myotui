using System.Collections.Generic;
namespace myotui.Models.Config
{
    public class SplitterBuffer : IBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<IBinding> Bindings {get; set;}
        public IEnumerable<IParameter> Parameters {get; set;}
    }
}