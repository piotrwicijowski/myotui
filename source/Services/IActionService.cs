using System;

namespace myotui.Services
{
    public interface IActionService
    {
        public Guid RegisterAction(string pattern, string scope, Func<string,bool> action);
        public void RemoveAction(Guid id);
        public void DispatchAction(string action, string currentScope);
    }
}