using System.Collections.Generic;
using myotui.Models;
using myotui.Models.Config;

namespace myotui.Services
{
    public interface INodeService
    {
        public ViewNode RootNode {get; set;}
        public ViewNode BuildNodeTree(IBuffer buffer, string scope, ViewNode parent = null, IDictionary<string,string> bufferParams = null, SizeHint width = null, SizeHint height = null);
        public ViewNode GetFocusedNode(ViewNode parentNode);
        public ViewNode GetNodeByScope(string scope, ViewNode startNode = null);
    }
}