using System.Collections.Generic;

namespace myotui.Models
{
    public class HBoxLayout : ILayout
    {
        public IEnumerable<IWindow> Windows {get; set;}
    }
}