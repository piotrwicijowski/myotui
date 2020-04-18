using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myotui;
using myotui.Services;
using myotui.Models.Config;
using Newtonsoft.Json;

namespace myotui.Services
{
    public class ActionListRawContentService : IRawContentService
    {
        protected readonly IParameterService _parameterService;
        public ActionListRawContentService(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }
        public string GetRawOutput(IValueContent content, IDictionary<string, string> parameters)
        {
            var listContent = content as ActionListValueContent;
            // var result = _parameterService.SubstituteParameters(string.Join(Environment.NewLine,listContent.Value),parameters);
            var result = listContent.Value.Select(element => new ActionListElement(){
                DisplayName = _parameterService.SubstituteParameters(element.DisplayName, parameters),
                Action = _parameterService.SubstituteParameters(element.Action, parameters),
            });
            
            return JsonConvert.SerializeObject(result);
        }

    }
}