using System.Collections.Generic;
namespace myotui.Models
{
    public class HBladeBuffer : IBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<ILayoutContent> Content {get; set;}
    }
}