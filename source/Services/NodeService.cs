using System;
using System.Collections.Generic;
using System.Linq;
using myotui.Models;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Services
{
    public class NodeService : INodeService
    {
        protected readonly IConfigurationService _configuration;
        protected readonly IParameterService _parameterService;
        public NodeService(IConfigurationService configuration, IParameterService parameterService)
        {
            _configuration = configuration;
            _parameterService = parameterService;
        }
        public ViewNode BuildNodeTree(IBuffer buffer, string scope, ViewNode parent = null, string bufferParams = null, SizeHint width = null, SizeHint height = null)
        {
            // var bufferCopy = buffer.Clone();
            var parameters = buffer.Parameters?.ToDictionary(parameter => parameter.Name, parameter => parameter.DefaultValue) ?? new Dictionary<string, string>();
            var parameterNames = buffer.Parameters?.Select(parameter => parameter.Name);
            var decodedBufferParams = _parameterService.DecodeParametersString(bufferParams, parameterNames?.ToList());
            decodedBufferParams.ToList().ForEach(pair =>
                {
                    try
                    {
                        parameters[pair.Key] = pair.Value;
                    }
                    catch(Exception _) { }
                }
            );
            var currentNode = new ViewNode()
            {
                Scope = scope,
                Buffer = buffer,
                Parent = parent,
                Width = width ?? new SizeHint(),
                Height = height ?? new SizeHint(),
                Parameters = parameters
            };
            if(buffer is ILayoutBuffer)
            {
                var layoutBuffer = buffer as ILayoutBuffer;
                currentNode.Children = layoutBuffer
                    .Windows
                    .Select(window => 
                        BuildNodeTree(
                            _configuration.GetBufferByName(window.Value),
                            $"{scope}/{window.Name}",
                            currentNode,
                            null,
                            window.Width,
                            window.Height
                            )
                    )
                    .ToList();
            }
            return currentNode;
        }

        public ViewNode GetFocusedNode(ViewNode parentNode)
        {
            var focusedChild = parentNode.Children?.FirstOrDefault(child => parentNode.View.Focused == child.View);
            if(focusedChild != null)
            {
                return GetFocusedNode(focusedChild);
            }
            return parentNode;
        }
        
    }
}