using System.Collections.Generic;

namespace myotui.Services
{
    public interface IContentMapService
    {
        public IEnumerable<object> MapRawData(string data);
    }
}