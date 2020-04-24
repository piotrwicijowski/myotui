using System.Collections.Generic;
using myotui.Models;
using Terminal.Gui;

namespace myotui.Services
{
    public interface IKeyService
    {
        public bool ProcessKeyEvent(KeyEvent keyEvent, ViewNode node);
        public void RegisterKeyActionTrigger(string trigger, List<string> actions, string bindingScope, string mode, ViewNode node);
        public void RemoveKeyActionTrigger(string trigger, List<string> actions, string bindingScope, string mode, ViewNode node);
        public void ClearStack();
    }
}