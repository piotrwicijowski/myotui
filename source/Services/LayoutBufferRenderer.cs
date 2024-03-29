using System;
using System.Collections.Generic;
using System.Linq;
using myotui.Models;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Services
{
    public abstract class LayoutBufferRenderer : BufferRenderer
    {
        protected LayoutBufferRenderer(IActionService actionService, IKeyService keyService, IBufferService bufferService, IModeService modeService) : base(actionService, keyService, bufferService, modeService)
        {
        }

        public override void RegisterActions(ViewNode node)
        {
            base.RegisterActions(node);
            RegisterFocusAction(node);
            RegisterOpenAction(node);
        }

        protected virtual void RegisterFocusAction(ViewNode node)
        {
            node.RegisteredActions.AddRange((new List<ActionRegistration>(){
                new ActionRegistration(){
                    Pattern = $"{node.Scope}.focus",
                    ActionScope = "/**",
                    Action = (_) => {node.Parent?.View.SetFocus(node.View);return true;}},
                }.Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "focusNext",
                        nodeScope : node.Scope,
                        action : (_) => _bufferService.FocusNextChild(node))
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "focusPrev",
                        nodeScope : node.Scope,
                        action : (_) => _bufferService.FocusPreviousChild(node))
                )
            ).Select(reg => _actionService.RegisterAction(reg)));
        }

        protected virtual void RegisterOpenAction(ViewNode node)
        {
            _actionService.RegisterAction($"{node.Scope}.open","/**",(parameters) => OpenAction(node, parameters));
            _actionService.RegisterAction($"{node.Scope}.replace","/**",(parameters) =>
            {
                _bufferService.CloseAllChildren(node);
                return OpenAction(node, parameters);
            });
            _actionService.RegisterAction($"{node.Scope}.replace_subsequent","/**",(parameters) =>
            {
                var focusedView = node.View.Focused;
                var (focusedNode, focusedIndex) = node.Children.Select((child, index) => (child, index)).Where(item => item.child.View == focusedView).FirstOrDefault();
                var nodesToClose = node.Children.Select((item, index) => (item, index)).Where(pair => pair.index > focusedIndex).Select(pair => pair.item).ToList();
                _bufferService.CloseBuffers(nodesToClose);
                return OpenAction(node, parameters);
            });
        }

        private bool OpenAction(ViewNode node, string parameters)
        {
            var parametersSplit = parameters.Split(" ");
            var bufferName = parametersSplit.FirstOrDefault();
            var bufferParameters = parametersSplit.Length <= 1 ? "" : string.Join(' ',parametersSplit.Skip(1));
            var hasFocusParameter = bool.TryParse(bufferParameters, out var focus);
            focus = hasFocusParameter ? focus : true;
            var newNode = _bufferService.OpenNewBuffer(node, bufferName, bufferParameters);
            node.View.SetFocus(newNode.View);
            return true;
        }

    }
}