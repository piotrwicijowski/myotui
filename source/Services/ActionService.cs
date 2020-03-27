using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace myotui.Services
{
    public class ActionService : IActionService
    {
        protected readonly List<ActionRegistration> _registeredActions = new List<ActionRegistration>();
        public void RegisterAction(string pattern, string scope, Func<bool> action)
        {
            _registeredActions.Add(new ActionRegistration
            {
                action = action,
                scope = scope,
                pattern = pattern,
            });
        }
        public void DispatchAction(string actionExpression, string currentScope)
        {
            _registeredActions
                .Where(reg => IsPatternMatching(actionExpression,reg.pattern))
                .Where(reg => IsInScope(actionExpression,reg.pattern))
                .OrderBy(reg => ScopeDepth(reg.scope))
                .Reverse()
                // .ToList()
                // .FirstOrDefault()?.action?.Invoke();
                .TakeWhile(reg =>
                {
                    var testreg = reg;
                 return !testreg.action.Invoke();
                }
                ).ToList();
        }

        private static bool IsPatternMatching(string actionExpression, string pattern)
        {
            var endToEndPattern = $"^{pattern}$";
            return Regex.IsMatch(actionExpression,endToEndPattern);
        }
        private static bool IsInScope(string currentScope, string actionScope)
        {
            //TODO create actual scope resolution
            return true;
        }

        private static int ScopeDepth(string scope)
        {
            //TODO create actual scope depth calculation
            return scope.Length;
        }

        protected class ActionRegistration
        {
            public Func<bool> action {get; set;}
            public string pattern {get; set;}
            public string scope {get; set;}
        }
    }

}