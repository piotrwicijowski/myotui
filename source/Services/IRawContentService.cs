using System.Collections.Generic;
using myotui.Models.Config;

namespace myotui.Services
{
    public interface IRawContentService
    {
        public dynamic GetRawOutput(IValueContent content, IDictionary<string, string> parameters);
    }
}