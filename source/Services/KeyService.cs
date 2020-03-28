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
        protected readonly INodeService _nodeService;
        public const string triggerPattern = "key ";
        public KeyService (IActionService actionService, INodeService nodeService)
        {
            _actionService = actionService;
            _nodeService = nodeService;
        }
        public bool ProcessKeyEvent (KeyEvent keyEvent, ViewNode node)
        {
            var focusedNode = _nodeService.GetFocusedNode(node);
            var checkedNode = focusedNode;
            while(checkedNode != null)
            {
                var isRegistered = checkedNode.TriggerActionDictionary.TryGetValue(keyEvent.Key, out var item);
                if(isRegistered)
                {
                    var (action, scope) = item;
                    _actionService.DispatchAction(action,focusedNode.Scope);
                    return true;
                }
                checkedNode = checkedNode.Parent;
            }
            return false;
        }

        public void RegisterKeyActionTrigger(string trigger, string action, ViewNode node)
        {
            var keys = DecodeKeySequence(trigger);
            if(keys.Any()){
                node.TriggerActionDictionary.Add(
                    keys.FirstOrDefault(), (action, node.Scope)
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