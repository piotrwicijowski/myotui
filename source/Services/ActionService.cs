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

        public void RegisterAction(string pattern, string scope, Func<bool> action)
        {
            _registeredActions.Add(new ActionRegistration
            {
                Action = action,
                ActionScope = scope,
                Pattern = pattern,
            });
        }
        public void DispatchAction(string actionExpression, string currentScope)
        {
            _registeredActions
                .Where(reg => IsPatternMatching(actionExpression,reg.Pattern))
                .Where(reg => _scopeService.IsInScope(currentScope,reg.ActionScope))
                .OrderBy(reg => _scopeService.ScopeDepth(reg.ActionScope))
                .Reverse()
                .TakeWhile(reg =>
                {
                    var testreg = reg;
                 return !testreg.Action.Invoke();
                }
                ).ToList();
        }

        private static bool IsPatternMatching(string actionExpression, string pattern)
        {
            var endToEndPattern = $"^{pattern}$";
            return Regex.IsMatch(actionExpression,endToEndPattern);
        }

        protected class ActionRegistration
        {
            public Func<bool> Action {get; set;}
            public string Pattern {get; set;}
            public string ActionScope {get; set;}
        }
    }

}