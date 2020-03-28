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
                .Where(reg => IsInScope(currentScope,reg.scope))
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
            var currentScopeDecoded = DecodeScopeString(currentScope);
            var actionScopeDecoded = DecodeScopeString(actionScope);
            var currentScopeEnumerator = currentScopeDecoded.GetEnumerator();
            var actionScopeEnumerator = actionScopeDecoded.GetEnumerator();
            currentScopeEnumerator.MoveNext();
            actionScopeEnumerator.MoveNext();
            var hasMore = true;
            var isWildcard = false;
            while(hasMore)
            {
                var currentValue = currentScopeEnumerator.Current;
                var actionValue = actionScopeEnumerator.Current;
                if(actionValue == "**")
                {
                    isWildcard = true;
                    var actionHasMore = actionScopeEnumerator.MoveNext();
                    if(!actionHasMore)
                    {
                        return true;
                    }
                    continue;
                }
                else if(isWildcard)
                {
                    if(currentValue == actionValue)
                    {
                        isWildcard = false;
                        continue;
                    }
                    var currentHasMore = currentScopeEnumerator.MoveNext();
                    if(!currentHasMore)
                    {
                        return false;
                    }
                    continue;
                }
                if(currentValue == actionValue || actionValue == "*")
                {
                    var currentHasMore = currentScopeEnumerator.MoveNext();
                    var actionHasMore = actionScopeEnumerator.MoveNext();
                    if(!currentHasMore && !actionHasMore)
                    {
                        return true;
                    }
                    // if(currentHasMore && actionHasMore)
                    // {
                        continue;
                    // }
                    // return false;
                }
                hasMore = false;
                isWildcard = false;
            } 
            return false;
        }

        private static int ScopeDepth(string scope)
        {
            //TODO create actual scope depth calculation
            return scope.Length;
        }

        private static IList<string> DecodeScopeString(string scope)
        {
            return scope.Split('/').Skip(1).ToList();
        }

        protected class ActionRegistration
        {
            public Func<bool> action {get; set;}
            public string pattern {get; set;}
            public string scope {get; set;}
        }
    }

}