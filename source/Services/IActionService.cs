using System;
using System.Collections.Generic;

namespace myotui.Services
{
    public interface IActionService
    {
        public Guid RegisterAction(string pattern, string scope, Func<string,bool> action);
        public IList<Guid> RegisterActionPair(string actionName, string nodeScope, Func<string,bool> action);
        public void RemoveAction(Guid id);
        public void DispatchAction(string action, string currentScope);
    }
}