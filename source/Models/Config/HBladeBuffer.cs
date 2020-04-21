using System.Collections.Generic;
namespace myotui.Models.Config
{
    public class HBladeBuffer : ILayoutBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<IBinding> Bindings {get; set;}
        public IEnumerable<IParameter> Parameters {get; set;}
        public IEnumerable<ILayoutContent> Windows {get; set;}
        public bool AutoSplitters {get; set;} = true;
        public bool Closable {get; set;} = false;
        public bool Focusable {get; set;} = true;
        public IValueContent Content {get; set;}
    }
}