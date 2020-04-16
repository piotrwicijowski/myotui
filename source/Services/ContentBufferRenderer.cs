using System;
using System.Collections.Generic;
using System.Linq;
using myotui.Models;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Services
{
    public class ContentBufferRenderer : BufferRenderer
    {
        protected ContentBufferRenderer(IActionService actionService, IKeyService keyService, IBufferService bufferService) : base(actionService, keyService, bufferService)
        {
        }
        public override void RegisterEvents(ViewNode node)
        {
            base.RegisterEvents(node);
            RegisterFocusAction(node);
        }

        protected void RegisterFocusAction(ViewNode node)
        {
            _actionService.RegisterAction($"{node.Scope}.focus","/**",(_) => {node.Parent?.View.SetFocus(node.View);return true;});
        }

        public override View Layout(ViewNode node)
        {
            return node.View;
        }
    }
}