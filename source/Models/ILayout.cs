using System.Collections.Generic;

namespace myotui.Models
{
    public interface ILayout
    {
        public IEnumerable<IWindow> Windows {get; set;}
    }
}