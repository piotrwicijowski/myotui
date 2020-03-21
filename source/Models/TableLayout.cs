using System.Collections.Generic;

namespace myotui.Models
{
    public class TableLayout : ILayout
    {
        public IEnumerable<IWindow> Windows {get; set;}
    }
}