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

        protected readonly IIndex<Type,IRawContentService> _rawContentServices;
        protected readonly IConfigurationService _configuration;
        protected readonly INodeService _nodeService;
        protected readonly IParameterService _parameterService;
        protected readonly IIndex<Type,IBufferRenderer> _bufferRenderers;
        protected readonly IIndex<ValueMapType,IContentMapService> _mapServices;
        public BufferService(IConfigurationService configuration, IIndex<Type,IBufferRenderer> bufferRenderers, INodeService nodeService, IParameterService parameterService, IIndex<Type,IRawContentService> rawContentServices, IIndex<ValueMapType,IContentMapService> mapServices)
        {
            _configuration = configuration;
            _bufferRenderers = bufferRenderers;
            _nodeService = nodeService;
            _parameterService = parameterService;
            _rawContentServices = rawContentServices;
            _mapServices = mapServices;
        }

        public View RenderNode(ViewNode node)
        {
            if(node.Buffer.Content != null)
            {
                var rawContentService = _rawContentServices[node.Buffer.Content.GetType()];
                var rawData = rawContentService.GetRawOutput(node, node.Parameters);
                dynamic acc = rawData;
                foreach(var map in node.Buffer.Content.Maps)
                {
                    var mapService = _mapServices[map];
                    acc = mapService.MapRawData(acc);
                }
                node.Data = acc;
            }
            var parentRenderer = _bufferRenderers[node.Buffer.GetType()];
            parentRenderer.Render(node);
            WireUpFocusSaving(node);
            node.Children?.ToList().ForEach(childNode => RenderNode(childNode));
            parentRenderer.RegisterActions(node);
            parentRenderer.RegisterBindings(node);
            return parentRenderer.Layout(node);
        }

        public ViewNode OpenNewBuffer(ViewNode parentNode, string bufferName, string bufferParams)
        {
            var buffer = _configuration.GetBufferByName(bufferName);

            var parameterNames = buffer.Parameters?.Select(parameter => parameter.Name);
            var decodedBufferParams = _parameterService.DecodeParametersString(bufferParams, parameterNames?.ToList());
            var newNode = _nodeService.BuildNodeTree(buffer, SuggestUniqueScope(parentNode, bufferName), parentNode, bufferParams: decodedBufferParams);
            if(parentNode.Children == null)
            {
                parentNode.Children = new List<ViewNode>();
            }
            parentNode.Children.Add(newNode);
            newNode.View = RenderNode(newNode);
            var parentRenderer = _bufferRenderers[parentNode.Buffer.GetType()];
            parentRenderer.Layout(parentNode);
            parentNode.View.LayoutSubviews();
            return newNode;
        }
         
        private string SuggestUniqueScope(ViewNode parentNode, string bufferName)
        {
            var baseScopeName = $"{parentNode.Scope}/{bufferName}";
            var suggestedName = baseScopeName;
            var childBufferScopes = parentNode.Children.Select(child => child.Scope).ToList();
            for(int i = 1; i <= 20; ++ i)
            {
                if(!childBufferScopes.Contains(suggestedName))
                {
                    break;
                }
                else
                {
                    suggestedName = $"{baseScopeName}_{i}";
                }
            }
            return suggestedName;
        }

        private void RemoveBindingsRecursive(ViewNode node)
        {
            if(node.Children != null){
                foreach(var child in node.Children)
                {
                    RemoveBindingsRecursive(child);
                }
            }
            var nodeRenderer = _bufferRenderers[node.Buffer.GetType()];
            nodeRenderer.RemoveBindings(node);
        }

        private void RemoveActionsRecursive(ViewNode node)
        {
            if(node.Children != null){
                foreach(var child in node.Children)
                {
                    RemoveActionsRecursive(child);
                }
            }
            var nodeRenderer = _bufferRenderers[node.Buffer.GetType()];
            nodeRenderer.RemoveActions(node);
        }

        public bool CloseAllChildren(ViewNode parentNode)
        {
            return CloseBuffers(parentNode.Children);
        }

        public bool CloseBuffers(IList<ViewNode> buffers)
        {
            var listCopy = new List<ViewNode>(buffers);
            foreach(var buffer in listCopy)
            {
                CloseBuffer(buffer);
            }
            return true;
        }

        public bool CloseBuffer(ViewNode node)
        {
            if(!node.Buffer.Closable)
            {
                return false;
            }
            var parentNode = node?.Parent;
            if(parentNode == null)
            {
                return true;
            }
            RemoveBindingsRecursive(node);
            RemoveActionsRecursive(node);

            parentNode.Children.Remove(node);
            var parentRenderer = _bufferRenderers[parentNode.Buffer.GetType()];
            parentRenderer.Layout(parentNode);
            parentNode.View.LayoutSubviews();
            var refocused = false;
            var nodeToRefocus = parentNode;
            while(!refocused && nodeToRefocus != null)
            {
                refocused = !refocused ? FocusPreviousChild(nodeToRefocus) : refocused;
                refocused = !refocused ? FocusNextChild(nodeToRefocus) : refocused;
                nodeToRefocus = nodeToRefocus.Parent;
            }
            return true;
        }

        public bool WireUpFocusSaving(ViewNode node)
        {
            if(node.View == null) {return false;}
            node.View.OnLeave += (sender, args) => 
            {
                node.LastFocusedNode = node.Children?.FirstOrDefault(child => child.View == node.View.Focused);
            };
            node.View.OnEnter += (sender, args) => 
            {
                node.SkipKeyHandling = false;
                if(node.LastFocusedNode != null && node.View.Subviews.Contains(node.LastFocusedNode.View))
                {
                    node.View.SetFocus(node.LastFocusedNode.View);
                }
            };
            return true;
        }
        public bool FocusNextChild(ViewNode node)
        {
            return ChangeFocusedChildByOffset(node, 1);
        }

        public bool FocusPreviousChild(ViewNode node)
        {
            return ChangeFocusedChildByOffset(node, -1);
        }
         
        private bool ChangeFocusedChildByOffset(ViewNode node, int offset)
        {
            if(!node.View.HasFocus) {return false;}
            if(node.Children == null || !node.Children.Any()) { return false; } 
            var visibleChildren = node.Children.Where(child => node.View.Subviews.Contains(child.View)).ToList();
            if(visibleChildren == null || !visibleChildren.Any()) { return false; } 
            var focusedChildIndex = visibleChildren.Select(child => child.View).ToList().IndexOf(node.View.Focused);
            for(int i = focusedChildIndex + offset; i <= visibleChildren.Count - 1 && i >= 0 ; i = i + offset)
            {
                var child = visibleChildren[i];
                if(child.Buffer.Focusable)
                {
                    node.View.SetFocus(child.View);
                    return true;
                }
            }
            return false;
        }

    }
}