using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myotui;
using myotui.Models;
using myotui.Models.Config;

namespace myotui.Services
{
    public class RefRawContentService : IRawContentService
    {
        protected readonly IParameterService _parameterService;
        protected readonly IScopeService _scopeService;
        protected readonly INodeService _nodeService;
        public RefRawContentService(IParameterService parameterService, IScopeService scopeService, INodeService nodeService)
        {
            _parameterService = parameterService;
            _scopeService = scopeService;
            _nodeService = nodeService;
        }
        public dynamic GetRawOutput(ViewNode node, IDictionary<string, string> parameters)
        {
            var refContent = node.Buffer.Content as RefValueContent;
            var resolvedScope = _scopeService.ResolveRelativeScope(node.Scope, refContent.Buffer);

            var referencedNode = _nodeService.GetNodeByScope(resolvedScope);
            return referencedNode.Data;
        }
    }
}