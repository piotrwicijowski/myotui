using System.Collections.Generic;

namespace myotui.Services
{
    public interface IContentMapService
    {
        public IEnumerable<IDictionary<string,object>> MapRawData(string data);
    }
}