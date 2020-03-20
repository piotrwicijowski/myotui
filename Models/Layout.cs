using System.Collections.Generic;

namespace myotui.Models
{
    public class Layout : ILayout
    {
        public LayoutType Type {get; set;}
        public IEnumerable<IWindow> Windows {get; set;}
    }
}