using System.Collections.Generic;
namespace myotui.Models.Config
{
    public class LayoutBuffer : Buffer
    {
        public IEnumerable<ILayoutContent> Windows {get; set;}
        public virtual bool AutoSplitters {get; set;} = true;
    }
}