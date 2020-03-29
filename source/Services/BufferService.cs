using System.Linq;
using Autofac;
using Terminal.Gui;
using myotui.Models.Config;
using Autofac.Features.Indexed;
using System;
using myotui.Models;
using System.Collections.Generic;

namespace myotui.Services
{
    public class BufferService : IBufferService
    {

        protected readonly IConfigurationService _configuration;
        protected readonly INodeService _nodeService;
        protected readonly IIndex<Type,IBufferRenderer> _bufferRenderers;
        public BufferService(IConfigurationService configuration, IIndex<Type,IBufferRenderer> bufferRenderers, INodeService nodeService)
        {
            _configuration = configuration;
            _bufferRenderers = bufferRenderers;
            _nodeService = nodeService;
        }

        public View RenderNode(ViewNode node)
        {
            var parentRenderer = _bufferRenderers[node.Buffer.GetType()];
            parentRenderer.Render(node);
            node.Children?.ToList().ForEach(childNode => RenderNode(childNode));
            parentRenderer.RegisterEvents(node);
            parentRenderer.RegisterBindings(node);
            return parentRenderer.Layout(node);
        }

        public void OpenNewBuffer(ViewNode parentNode, string bufferName, string bufferParams)
        {
            var buffer = _configuration.GetBufferByName(bufferName);
            var newNode = _nodeService.BuildNodeTree(buffer, $"{parentNode.Scope}/{buffer.Name}", parentNode, bufferParams: bufferParams);
            newNode.View = RenderNode(newNode);
            if(parentNode.Children == null)
            {
                parentNode.Children = new List<ViewNode>();
            }
            parentNode.Children.Add(newNode);
            var parentRenderer = _bufferRenderers[parentNode.Buffer.GetType()];
            parentRenderer.Layout(parentNode);
            parentNode.View.LayoutSubviews();
        }
         
    }
}