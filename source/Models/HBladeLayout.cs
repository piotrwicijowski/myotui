using System.Collections.Generic;

namespace myotui.Models
{
    public class HBladeLayout : ILayout
    {
        public IEnumerable<IWindow> Windows {get; set;}
    }
}