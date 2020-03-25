using System.Collections.Generic;
namespace myotui.Models.Config
{
    public interface ILayoutBuffer : IBuffer
    {
        public IEnumerable<ILayoutContent> Windows {get; set;}
    }
}