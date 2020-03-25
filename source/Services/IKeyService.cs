using Terminal.Gui;

namespace myotui.Services
{
    public interface IKeyService
    {
        public bool ProcessKeyEvent(KeyEvent keyEvent);
        public void RegisterKeyActionTrigger(string trigger, string action, string scope);
    }
}