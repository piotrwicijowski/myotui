using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myotui.Services
{
    public class NopMapService : IContentMapService
    {
        public dynamic MapRawData(dynamic data)
        {
            return data;
        }
    }
}