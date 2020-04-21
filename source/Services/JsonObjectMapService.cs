using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myotui.Services
{
    public class JsonObjectMapService : IContentMapService
    {
        public dynamic MapRawData(dynamic data)
        {
            var stringData = data as string;
            var json = JsonConvert.DeserializeObject<IDictionary<string,object>>(stringData);
            return json;
        }
    }
}