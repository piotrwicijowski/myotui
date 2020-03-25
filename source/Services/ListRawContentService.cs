using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myotui;
using myotui.Models.Config;

namespace myotui.Services
{
    public class ListRawContentService : IRawContentService
    {
        public string GetRawOutput(IValueContent content)
        {
            var listContent = content as ListValueContent;
            return string.Join(Environment.NewLine,listContent.Value);
        }

    }
}