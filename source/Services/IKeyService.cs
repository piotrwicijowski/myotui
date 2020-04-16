using myotui.Models;
using Terminal.Gui;

namespace myotui.Services
{
    public interface IKeyService
    {
        public bool ProcessKeyEvent(KeyEvent keyEvent, ViewNode node);
        public void RegisterKeyActionTrigger(string trigger, string action, string bindingScope, ViewNode node);

        public void RemoveKeyActionTrigger(string trigger, string action, string bindingScope, ViewNode node);
    }
}