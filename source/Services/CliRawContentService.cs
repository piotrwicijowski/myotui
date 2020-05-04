using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Autofac.Features.AttributeFilters;
using myotui;
using myotui.Models;
using myotui.Models.Config;

namespace myotui.Services
{
    public class CliRawContentService : IRawContentService
    {
        protected readonly IParameterService _parameterService;
        protected readonly IIOService _cliIOService;
        public CliRawContentService(IParameterService parameterService, [KeyFilter(IOServiceType.Cli)]IIOService cliIOService)
        {
            _parameterService = parameterService;
            _cliIOService = cliIOService;
        }
        public dynamic GetRawOutput(ViewNode node, IDictionary<string, string> parameters)
        {
            var cliContent = node.Buffer.Content as CliValueContent;
            var cliSubstituted = _parameterService.SubstituteParameters(string.Join(Environment.NewLine,cliContent.Input),parameters);
            return _cliIOService.Run(cliSubstituted);
        }

    }
}