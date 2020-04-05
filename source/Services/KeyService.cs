using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using myotui.Models;
using Terminal.Gui;

namespace myotui.Services {
    public class KeyService : IKeyService {
        //TODO this is only for demonstration purposes
        protected readonly IActionService _actionService;
        protected readonly IScopeService _scopeService;
        protected readonly INodeService _nodeService;
        protected readonly IParameterService _parameterService;
        public const string triggerPattern = "key ";
        public KeyService (IActionService actionService, INodeService nodeService, IScopeService scopeService, IParameterService parameterService)
        {
            _actionService = actionService;
            _nodeService = nodeService;
            _scopeService = scopeService;
            _parameterService = parameterService;
        }
        public bool ProcessKeyEvent (KeyEvent keyEvent, ViewNode node)
        {
            var focusedNode = _nodeService.GetFocusedNode(node);
            var checkedNode = focusedNode;
            while(checkedNode != null)
            {
                var isRegistered = checkedNode.TriggerActionDictionary.TryGetValue(keyEvent.Key, out var item);
                checkedNode = checkedNode.Parent;
                if(isRegistered)
                {
                    var (action, bindingScope) = item;
                    if(!_scopeService.IsInScope(focusedNode.Scope,bindingScope))
                    {
                        continue;
                    }
                    var parametrizedAction = _parameterService.SubstituteParameters(action, focusedNode.Parameters);
                    _actionService.DispatchAction(parametrizedAction,focusedNode.Scope);
                    return true;
                }
            }
            return false;
        }

        public void RegisterKeyActionTrigger(string trigger, string action, string bindingScope, ViewNode node)
        {
            var keys = DecodeKeySequence(trigger);
            if(keys.Any()){
                node.TriggerActionDictionary.Add(
                    keys.FirstOrDefault(), (_scopeService.ResolveRelativeAction(node.Scope, action), _scopeService.ResolveRelativeScope(node.Scope,bindingScope))
                );
            }
        }


        private List<Key> DecodeKeySequence(string trigger)
        {
            var matches = Regex.Matches(
                    trigger.Substring(triggerPattern.Length),
                    @"(<([^<>]+)>)|([^<> ])"
                )
                .Select(
                    match => 
                    {
                        var value = match.Value;
                        var isSpecial = match.Groups[2].Success;
                        var isSpecialCorrect = Enum.TryParse<Key>(match.Groups[2].Value, out var specialKey);
                        return (isSpecial && isSpecialCorrect) ? specialKey : (Key)(value[0]);
                    }
                ).ToList();
            return matches;
        }
    }
}