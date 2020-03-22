using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myotui.Services
{
    public class JsonArrayMapService : IContentMapService
    {
        public IEnumerable<object> MapRawData(string data)
        {
            var json = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(data);
            return json.Select(x => x.name);
        }
    }
}