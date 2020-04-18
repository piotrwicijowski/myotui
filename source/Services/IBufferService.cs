using Terminal.Gui;
using myotui.Models.Config;
using myotui.Models;
using System.Collections.Generic;

namespace myotui.Services
{
    public interface IBufferService
    {
        public View RenderNode(ViewNode node);
         
        public ViewNode OpenNewBuffer(ViewNode parentNode, string bufferName, string bufferParams);

        public bool CloseBuffer(ViewNode node);

        public bool CloseAllChildren(ViewNode parentNode);

        public bool CloseBuffers(IList<ViewNode> buffers);

    }
}