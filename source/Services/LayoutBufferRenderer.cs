using System;
using System.Collections.Generic;
using System.Linq;
using myotui.Models;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Services
{
    public class LayoutBufferRenderer : BufferRenderer
    {
        protected LayoutBufferRenderer(IActionService actionService, IKeyService keyService, IBufferService bufferService) : base(actionService, keyService, bufferService)
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
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focus","/**",(_) => {node.Parent?.View.SetFocus(node.View);return true;}));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focusNext",node.Scope,(_) => node.FocusNextChild()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focusPrev",node.Scope,(_) => node.FocusPreviousChild()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/focusNext",$"{node.Scope}/**",(_) => node.FocusNextChild()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/focusPrev",$"{node.Scope}/**",(_) => node.FocusPreviousChild()));
        }

        protected virtual void RegisterOpenAction(ViewNode node)
        {
            _actionService.RegisterAction($"{node.Scope}.open","/**",(parameters) => {

                var parametersSplit = parameters.Split(" ");
                var bufferName = parametersSplit.FirstOrDefault();
                var bufferParameters = parametersSplit.Length <= 1 ? "" : string.Join(' ',parametersSplit.Skip(1));
                var hasFocusParameter = bool.TryParse(bufferParameters, out var focus);
                focus = hasFocusParameter ? focus : true;
                var newNode = _bufferService.OpenNewBuffer(node, bufferName, bufferParameters);
                node.View.SetFocus(newNode.View);
                return true;
            });
        }
    }
}