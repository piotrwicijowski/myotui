using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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
        protected readonly Stack<Key> _keyStack = new Stack<Key>();
        protected readonly KeyPrefixDictionary _keyPrefixDictionary;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public KeyService (IActionService actionService, INodeService nodeService, IScopeService scopeService, IParameterService parameterService, KeyPrefixDictionary keyPrefixDictionary)
        {
            _actionService = actionService;
            _nodeService = nodeService;
            _scopeService = scopeService;
            _parameterService = parameterService;
            _keyPrefixDictionary = keyPrefixDictionary;
        }
        public bool ProcessKeyEvent (KeyEvent keyEvent, ViewNode node)
        {
            var focusedNode = _nodeService.GetFocusedNode(node);
            var checkedNode = focusedNode;
            _keyStack.Push(keyEvent.Key);
            while(checkedNode != null)
            {
                if(checkedNode.SkipKeyHandling)
                {
                    ClearStack();
                    return false;
                }
                var allActionsForPrefix = _keyPrefixDictionary.GetAllActionsByKeyPrefix(_keyStack.Reverse().ToList(),checkedNode.Scope);
                if(allActionsForPrefix == null || allActionsForPrefix.Count == 0)
                {
                    checkedNode = checkedNode.Parent;
                    continue;
                }
                else if(allActionsForPrefix.Count == 1)
                {
                    var action = allActionsForPrefix.FirstOrDefault();
                    var parametrizedAction = _parameterService.SubstituteParameters(action, focusedNode.Parameters);
                    parametrizedAction = _parameterService.SubstituteParameters(parametrizedAction, checkedNode.Parameters);
                    _actionService.DispatchAction(parametrizedAction, checkedNode.Scope);
                    _keyStack.Clear();
                    return true;
                }
                else
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource = new CancellationTokenSource();
                    var cancellationToken = _cancellationTokenSource.Token;
                    Task.Delay(2000, cancellationToken).ContinueWith((t) =>
                    {
                        if(!cancellationToken.IsCancellationRequested)
                        {
                            ClearStack();
                        }
                    }, cancellationToken);
                    return true;
                }
            }
            _keyStack.Clear();
            return false;
        }

        public void RegisterKeyActionTrigger(string trigger, string action, string bindingScope, ViewNode node)
        {
            var keys = DecodeKeySequence(trigger);
            var resolvedAction = _scopeService.ResolveRelativeAction(node.Scope, action);
            var resolvedScope = _scopeService.ResolveRelativeScope(node.Scope, bindingScope);
            _keyPrefixDictionary.AddAction(keys, resolvedAction, resolvedScope);
        }

        public void RemoveKeyActionTrigger(string trigger, string action, string bindingScope, ViewNode node)
        {
            var keys = DecodeKeySequence(trigger);
            var resolvedAction = _scopeService.ResolveRelativeAction(node.Scope, action);
            var resolvedScope = _scopeService.ResolveRelativeScope(node.Scope, bindingScope);
            _keyPrefixDictionary.RemoveAction(keys, resolvedAction, resolvedScope);
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

        public void ClearStack()
        {
            _keyStack.Clear();
        }
    }
}