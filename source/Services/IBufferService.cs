using Terminal.Gui;
using myotui.Models.Config;
using myotui.Models;

namespace myotui.Services
{
    public interface IBufferService
    {
        public View RenderNode(ViewNode node);
         
        public void OpenNewBuffer(ViewNode parentNode, string bufferName, string bufferParams);

        public bool CloseBuffer(ViewNode node);
    }
}