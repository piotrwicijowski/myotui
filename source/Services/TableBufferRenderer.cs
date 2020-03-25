using System.Linq;
using Terminal.Gui;
using myotui.Models.Config;
using myotui.Services;
using Autofac.Features.Indexed;
using System;
using myotui.Models;

namespace myotui.Services
{
    public class TableBufferRenderer : IBufferRenderer
    {
        protected readonly IActionService _actionService;
        protected readonly IIndex<Type,IRawContentService> _rawContentServices;
        protected readonly IIndex<ValueMapType,IContentMapService> _maps;
        public TableBufferRenderer(IActionService actionService, IIndex<Type,IRawContentService> rawContentServices, IIndex<ValueMapType,IContentMapService> maps)
        {
            _actionService = actionService;
            _rawContentServices = rawContentServices;
            _maps = maps;
        }

        public View Layout(ViewNode node)
        {
            return node.View;
        }

        public void RegisterBindings(ViewNode node)
        {
            return;
        }

        public void RegisterEvents(ViewNode node)
        {
            if(node.Parent != null)
            {
                RegisterFocusAction(node.Parent.View,node.View,node.Scope);
            }
        }

        public View Render(ViewNode node)
        {
            var buffer = node.Buffer;
            var scope = node.Scope;
            var tablebuffer = buffer as TableBuffer;
            var view = new FrameView("");
            var rawContentService = _rawContentServices[tablebuffer.Content.GetType()];
            var rawContent = rawContentService.GetRawOutput(tablebuffer.Content);
            var map = _maps[tablebuffer.Content.Map];
            var content = map.MapRawData(rawContent);
            var listView = new ListView(content.ToList());
            listView.X = 0;
            listView.Y = 0;
            listView.Width = Dim.Fill();
            listView.Height = Dim.Fill();
            view.Add(listView);
            node.View = view;
            return view;
        }
        protected void RegisterFocusAction(View parentView, View childView, string scope)
        {
            _actionService.RegisterAction($"{scope}.focus",scope,() => parentView.SetFocus(childView));
        }
        
    }
}