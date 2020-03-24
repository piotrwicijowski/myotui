using Terminal.Gui;

namespace myotui.Services
{
    public interface IKeyService
    {
        public void ProcessKeyEvent(KeyEvent keyEvent);
        public void RegisterKeyActionTrigger(string trigger, string action, string scope);
    }
}