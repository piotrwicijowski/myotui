using Terminal.Gui;

namespace myotui.Services
{
    public interface IKeyCodeMap
    {
        public Key GetPhysicalKeyCode(string logicalKeyName);
         
    }
}