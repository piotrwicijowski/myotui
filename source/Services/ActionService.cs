using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace myotui.Services
{
    public class ActionService : IActionService
    {
        protected readonly List<ActionRegistration> _registeredActions = new List<ActionRegistration>();
        protected readonly IScopeService _scopeService;

        public ActionService(IScopeService scopeService)
        {
            _scopeService = scopeService;
        }

        public Guid RegisterAction(string pattern, string scope, Func<string,bool> action)
        {
            var registrationId = Guid.NewGuid();
            _registeredActions.Add(new ActionRegistration
            {
                Action = action,
                ActionScope = scope,
                Pattern = pattern,
                Id = registrationId
            });
            return registrationId;
        }

        public void RemoveAction(Guid id)
        {
            _registeredActions.RemoveAll(registration => registration.Id == id);
        }

        public void DispatchAction(string actionExpression, string currentScope)
        {
            var actionSplit = actionExpression.Split(" ");
            var actionName = actionSplit.FirstOrDefault();
            var actionParameters = actionSplit.Length <= 1 ? "" : string.Join(' ',actionSplit.Skip(1));
            _registeredActions
                .Where(reg => IsPatternMatching(actionName,reg.Pattern))
                .Where(reg => _scopeService.IsInScope(currentScope,reg.ActionScope))
                .OrderBy(reg => _scopeService.ScopeDepth(reg.ActionScope))
                .Reverse()
                .TakeWhile(reg => !reg.Action.Invoke(actionParameters))
                .ToList();
        }

        private static bool IsPatternMatching(string actionExpression, string pattern)
        {
            var endToEndPattern = $"^{pattern}$";
            return Regex.IsMatch(actionExpression,endToEndPattern);
        }

        protected class ActionRegistration
        {
            public Guid Id {get; set;}
            public Func<string,bool> Action {get; set;}
            public string Pattern {get; set;}
            public string ActionScope {get; set;}
        }
    }

}