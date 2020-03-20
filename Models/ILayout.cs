using System.Collections.Generic;

namespace myotui.Models
{
    public interface ILayout
    {
        public LayoutType Type {get; set;}
        public IEnumerable<IWindow> Windows {get; set;}
    }
}