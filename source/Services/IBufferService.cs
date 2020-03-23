using Terminal.Gui;
namespace myotui.Services
{
    public interface IBufferService
    {
        public View RenderBuffer(string name, string scope);
         
    }
}