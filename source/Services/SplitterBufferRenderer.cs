using System.Linq;
using Terminal.Gui;
using myotui.Models.Config;
using myotui.Services;
using Autofac.Features.Indexed;
using System;
using myotui.Models;
using myotui.Views;

namespace myotui.Services
{
    public class SplitterBufferRenderer : ContentBufferRenderer
    {
        protected SplitterBufferRenderer(IActionService actionService, IKeyService keyService, IBufferService bufferService, IModeService modeService) : base(actionService, keyService, bufferService, modeService)
        {
        }

        public override View Render(ViewNode node)
        {
            var view = new Splitter();
            node.View = view;
            return view;
        }
    }
}