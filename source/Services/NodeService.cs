using System.Collections.Generic;
using System.Linq;
using myotui.Models;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Services
{
    public class NodeService : INodeService
    {
        protected readonly IBufferService _bufferSerivce;
        public NodeService(IBufferService bufferService)
        {
            _bufferSerivce = bufferService;
        }
        public ViewNode BuildNodeTree(IBuffer buffer, string scope, ViewNode parent = null)
        {
            var currentNode = new ViewNode()
            {
                Scope = scope,
                Buffer = buffer,
                Parent = parent,
            };
            if(buffer is ILayoutBuffer)
            {
                var layoutBuffer = buffer as ILayoutBuffer;
                currentNode.Children = layoutBuffer
                    .Windows
                    .Select(window => 
                        BuildNodeTree(_bufferSerivce.GetBufferByName(window.Value), $"{scope}/{window.Name}", currentNode)
                    )
                    .ToList();
            }
            return currentNode;
               
        }

        // public View RenderNode(ViewNode node)
        // {
        //     var childViews = node.Children?.Select(child => RenderNode(child));
        //     return _bufferSerivce.RenderBuffer(node.Buffer, node.Scope, childViews);
        // }
    }
}