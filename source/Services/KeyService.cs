﻿using System;
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
        protected readonly IKeyCodeMap _keyCodeMap;
        public const string triggerPattern = "key ";
        protected readonly Stack<Key> _keyStack = new Stack<Key>();
        protected readonly KeyPrefixDictionary _keyPrefixDictionary;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public KeyService(IActionService actionService, INodeService nodeService, IScopeService scopeService, IParameterService parameterService, KeyPrefixDictionary keyPrefixDictionary, IKeyCodeMap keyCodeMap)
        {
            _actionService = actionService;
            _nodeService = nodeService;
            _scopeService = scopeService;
            _parameterService = parameterService;
            _keyPrefixDictionary = keyPrefixDictionary;
            _keyCodeMap = keyCodeMap;
        }
        public bool ProcessKeyEvent (KeyEvent keyEvent, ViewNode node)
        {
            var focusedNode = _nodeService.GetFocusedNode(node);
            var checkedNode = focusedNode;
            _keyStack.Push(keyEvent.Key);
            while(checkedNode != null)
            {
                var allActionsForPrefix = _keyPrefixDictionary.GetAllActionsByKeyPrefix(_keyStack.Reverse().ToList(),checkedNode.Scope);
                if(allActionsForPrefix == null || allActionsForPrefix.Count == 0)
                {
                    checkedNode = checkedNode.Parent;
                    continue;
                }
                else if(allActionsForPrefix.Count == 1)
                {
                    var actions = allActionsForPrefix.FirstOrDefault();
                    foreach(var action in actions)
                    {
                        var parametrizedAction = _parameterService.SubstituteParameters(action, focusedNode.Parameters);
                        parametrizedAction = _parameterService.SubstituteParameters(parametrizedAction, checkedNode.Parameters);
                        _actionService.DispatchAction(parametrizedAction, checkedNode.Scope);
                    }
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

        public void RegisterKeyActionTrigger(string trigger, List<string> actions, string bindingScope, string mode, ViewNode node)
        {
            var keys = DecodeKeySequence(trigger);
            var resolvedActions = actions.Select(action => _scopeService.ResolveRelativeAction(node.Scope, action)).ToList();
            var resolvedScope = _scopeService.ResolveRelativeScope(node.Scope, bindingScope);
            _keyPrefixDictionary.AddAction(keys, resolvedActions, resolvedScope, mode);
        }

        public void RemoveKeyActionTrigger(string trigger, List<string> actions, string bindingScope, string mode, ViewNode node)
        {
            var keys = DecodeKeySequence(trigger);
            var resolvedActions = actions.Select(action => _scopeService.ResolveRelativeAction(node.Scope, action)).ToList();
            var resolvedScope = _scopeService.ResolveRelativeScope(node.Scope, bindingScope);
            _keyPrefixDictionary.RemoveAction(keys, resolvedActions, resolvedScope, mode);
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
                        // var isSpecialCorrect = Enum.TryParse<Key>(match.Groups[2].Value, out var specialKey);
                        return _keyCodeMap.GetPhysicalKeyCode(isSpecial ? match.Groups[2].Value : value);
                        // return (isSpecial && isSpecialCorrect) ? specialKey : (Key)(value[0]);
                    }
                )
                // .Select(logicalKey => _keyCodeMap.GetPhysicalKeyCode(logicalKey))
                .ToList();
            return matches;
        }

        public void ClearStack()
        {
            _keyStack.Clear();
        }
    }
}