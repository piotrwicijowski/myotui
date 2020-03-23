using System.Collections.Generic;
namespace myotui.Models
{
    public interface ILayoutBuffer : IBuffer
    {
        public IEnumerable<ILayoutContent> Windows {get; set;}
    }
}