using System.Collections.Generic;

namespace myotui.Models
{
    public class VBoxLayout : ILayout
    {
        public IEnumerable<IWindow> Windows {get; set;}
    }
}