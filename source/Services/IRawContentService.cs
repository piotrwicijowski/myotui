using System.Collections.Generic;
using myotui.Models;

namespace myotui.Services
{
    public interface IRawContentService
    {
        public string GetRawOutput(IValueContent content);
    }
}