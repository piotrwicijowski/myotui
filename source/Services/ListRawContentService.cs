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
        public dynamic GetRawOutput(IValueContent content, IDictionary<string, string> parameters)
        {
            var listContent = content as ListValueContent;
            return listContent.Value.Select(val => _parameterService.SubstituteParameters(val, parameters)).ToList();
        }

    }
}