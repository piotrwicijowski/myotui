using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myotui.Services
{
    public class JsonArrayMapService : IContentMapService
    {
        public IEnumerable<IDictionary<string,object>> MapRawData(string data)
        {
            var json = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string,object>>>(data);
            // var json = jsonObjects.Select(dict => )

            return json;
        }
    }
}