using myotui.Models;
using Terminal.Gui;

namespace myotui.Services
{
    public interface IKeyService
    {
        public bool ProcessKeyEvent(KeyEvent keyEvent, ViewNode node);
        public void RegisterKeyActionTrigger(string trigger, string action, ViewNode node);
    }
}