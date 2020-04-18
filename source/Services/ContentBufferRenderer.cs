using System;
using System.Collections.Generic;
using System.Linq;
using myotui.Models;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Services
{
    public abstract class ContentBufferRenderer : BufferRenderer
    {
        protected ContentBufferRenderer(IActionService actionService, IKeyService keyService, IBufferService bufferService) : base(actionService, keyService, bufferService)
        {
        }
        public override void RegisterActions(ViewNode node)
        {
            base.RegisterActions(node);
            RegisterFocusAction(node);
        }

        protected void RegisterFocusAction(ViewNode node)
        {
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focus","/**",(_) => {node.Parent?.View.SetFocus(node.View);return true;}));
        }

        public override View Layout(ViewNode node)
        {
            return node.View;
        }
    }
}