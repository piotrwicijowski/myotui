using System.Collections.Generic;

namespace myotui.Models
{
    public class VBladeLayout : ILayout
    {
        public IEnumerable<IWindow> Windows {get; set;}
    }
}