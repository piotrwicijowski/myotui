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
        public NodeService(IConfigurationService configuration)
        {
            _configuration = configuration;
        }
        public ViewNode BuildNodeTree(IBuffer buffer, string scope, ViewNode parent = null, SizeHint width = null, SizeHint height = null)
        {
            var currentNode = new ViewNode()
            {
                Scope = scope,
                Buffer = buffer,
                Parent = parent,
                Width = width ?? new SizeHint(),
                Height = height ?? new SizeHint(),
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
        // public View RenderNode(ViewNode node)
        // {
        //     var childViews = node.Children?.Select(child => RenderNode(child));
        //     return _bufferSerivce.RenderBuffer(node.Buffer, node.Scope, childViews);
        // }
    }
}