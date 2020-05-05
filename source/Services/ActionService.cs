using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using myotui.Models;

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

        public IList<Guid> RegisterActionPair(string actionName, string nodeScope, Func<string,bool> action)
        {
            var result = new List<Guid>();
            result.Add(RegisterAction($"{nodeScope}.{actionName}","/**",action));
            result.Add(RegisterAction($"/{actionName}",$"{nodeScope}/**",action));
            return result;
        }
        public Guid RegisterAction(ActionRegistration registration)
        {
            _registeredActions.Add(registration);
            return registration.Id;
        }
        public IList<Guid> RegisterActions(IList<ActionRegistration> registrations)
        {
            _registeredActions.AddRange(registrations);
            return registrations.Select(reg => reg.Id).ToList();
        }
        public Guid RegisterAction(string pattern, string scope, Func<string,bool> action)
        {
            var registration = new ActionRegistration
            {
                Action = action,
                ActionScope = scope,
                Pattern = pattern
            };
            _registeredActions.Add(registration);
            return registration.Id;
        }

        public void RemoveAction(Guid id)
        {
            _registeredActions.RemoveAll(registration => registration.Id == id);
        }

        public void DispatchAction(string actionExpression, string currentScope)
        {
            var actionSplit = actionExpression.Split(" ");
            var actionName = actionSplit.FirstOrDefault();
            var absoluteActionName = _scopeService.ResolveRelativeAction(currentScope,actionName);
            var actionParameters = actionSplit.Length <= 1 ? "" : string.Join(' ',actionSplit.Skip(1));
            _registeredActions
                .Where(reg => IsPatternMatching(absoluteActionName,reg.Pattern))
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
    }

}