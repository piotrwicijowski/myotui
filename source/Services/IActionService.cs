using System;
using System.Collections.Generic;
using myotui.Models;

namespace myotui.Services
{
    public interface IActionService
    {
        public Guid RegisterAction(ActionRegistration registration);
        public IList<Guid> RegisterActions(IList<ActionRegistration> registrations);
        public Guid RegisterAction(string pattern, string scope, Func<string,bool> action);
        public IList<Guid> RegisterActionPair(string actionName, string nodeScope, Func<string,bool> action);
        public void RemoveAction(Guid id);
        public void DispatchAction(string action, string currentScope);
    }
}