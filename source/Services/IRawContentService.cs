using System.Collections.Generic;
using myotui.Models;
using myotui.Models.Config;

namespace myotui.Services
{
    public interface IRawContentService
    {
        public dynamic GetRawOutput(ViewNode node, IDictionary<string, string> parameters);
    }
}