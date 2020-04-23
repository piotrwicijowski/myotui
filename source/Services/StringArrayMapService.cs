using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace myotui.Services
{
    public class StringArrayMapService : IContentMapService
    {
        public dynamic MapRawData(dynamic data)
        {
            var listData = data as List<string>;
            return listData.Select(value => new Dictionary<string,object>(){{"value", value}} as IDictionary<string,object>).ToList();
        }
    }
}