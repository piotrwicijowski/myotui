
using myotui.Models;
using Terminal.Gui;

namespace myotui.Services
{
    public interface IBufferRenderer
    {
        public View Render(IBuffer buffer);
    }
}