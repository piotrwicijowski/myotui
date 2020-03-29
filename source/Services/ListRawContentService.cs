using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myotui;
using myotui.Services;
using myotui.Models.Config;

namespace myotui.Services
{
    public class ListRawContentService : IRawContentService
    {
        protected readonly IParameterService _parameterService;
        public ListRawContentService(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }
        public string GetRawOutput(IValueContent content, IDictionary<string, string> parameters)
        {
            var listContent = content as ListValueContent;
            return _parameterService.SubstituteParameters(string.Join(Environment.NewLine,listContent.Value),parameters);
        }

    }
}