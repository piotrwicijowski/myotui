using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace myotui.Services
{
    public class StringArrayMapService : IContentMapService
    {
        public IEnumerable<object> MapRawData(string data)
        {
            return data.Split(Environment.NewLine);
        }
    }
}