using System;

namespace myotui.Services
{
    public interface IActionService
    {
        public void RegisterAction(string pattern, string scope, Func<string,bool> action);
        public void DispatchAction(string action, string currentScope);
    }
}