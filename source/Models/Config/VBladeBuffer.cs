using System.Collections.Generic;
namespace myotui.Models.Config
{
    public class VBladeBuffer : ILayoutBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<IBinding> Bindings {get; set;}
        public IEnumerable<ILayoutContent> Windows {get; set;}
    }
}