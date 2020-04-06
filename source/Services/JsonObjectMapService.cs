using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myotui.Services
{
    public class JsonObjectMapService : IContentMapService
    {
        public IEnumerable<IDictionary<string,object>> MapRawData(string data)
        {
            var json = JsonConvert.DeserializeObject<IDictionary<string,object>>(data);
            return new List<IDictionary<string,object>>(){json};
        }
    }
}