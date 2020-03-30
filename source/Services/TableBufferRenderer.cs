using System.Linq;
using Terminal.Gui;
using myotui.Models.Config;
using myotui.Services;
using Autofac.Features.Indexed;
using System;
using myotui.Models;
using myotui.Views;
using System.Collections.Generic;

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
                RegisterFocusAction(node);
            }
        }

        // public View Render(ViewNode node)
        // {
        //     var buffer = node.Buffer;
        //     var scope = node.Scope;
        //     var tablebuffer = buffer as TableBuffer;
        //     var view = new View();
        //     var rawContentService = _rawContentServices[tablebuffer.Content.GetType()];
        //     var rawContent = rawContentService.GetRawOutput(tablebuffer.Content, node.Parameters);
        //     var map = _maps[tablebuffer.Content.Map];
        //     var content = map.MapRawData(rawContent)?.ToList();
        //     content = content ?? new List<object>();
        //     var listView = new ListView(content);
        //     listView.X = 0;
        //     listView.Y = 0;
        //     listView.Width = Dim.Fill();
        //     listView.Height = Dim.Fill();
        //     view.Add(listView);
        //     node.View = view;
        //     return view;
        // }
        
        public View Render(ViewNode node)
        {
            var buffer = node.Buffer;
            var scope = node.Scope;
            var tablebuffer = buffer as TableBuffer;
            var rawContentService = _rawContentServices[tablebuffer.Content.GetType()];
            var rawContent = rawContentService.GetRawOutput(tablebuffer.Content, node.Parameters);
            var map = _maps[tablebuffer.Content.Map];
            var content = map.MapRawData(rawContent)?.ToList();
            content = content ?? new List<IDictionary<string,object>>();
            //TODO
            var columnMapOrder = content?.FirstOrDefault()?.Keys.Take(3).ToList();
            var columnWidths = columnMapOrder.Select(x => 1.0).ToList();
            var tableData = new TableData(content,columnMapOrder, columnWidths);
            var view = new TableView(tableData);
            // view.X = 0;
            // view.Y = 0;
            // view.Width = Dim.Fill();
            // view.Height = Dim.Fill();
            node.View = view;
            return view;
        }
        
        protected void RegisterFocusAction(ViewNode node)
        {
            _actionService.RegisterAction($"{node.Scope}.focus","/**",(_) => {node.Parent?.View.SetFocus(node.View);return true;});
        }
        
    }
}