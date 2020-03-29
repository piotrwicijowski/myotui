using myotui.Models;
using myotui.Models.Config;

namespace myotui.Services
{
    public interface INodeService
    {
        public ViewNode BuildNodeTree(IBuffer buffer, string scope, ViewNode parent = null, SizeHint width = null, SizeHint height = null);
        public ViewNode GetFocusedNode(ViewNode parentNode);
    }
}