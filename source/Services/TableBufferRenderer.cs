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
    public class TableBufferRenderer : ContentBufferRenderer
    {
        protected readonly IIndex<Type,IRawContentService> _rawContentServices;
        protected readonly IIndex<ValueMapType,IContentMapService> _maps;
        public TableBufferRenderer(IActionService actionService, IIndex<Type,IRawContentService> rawContentServices, IIndex<ValueMapType,IContentMapService> maps, IBufferService bufferService, IKeyService keyService) : base(actionService, keyService, bufferService)
        {
            _rawContentServices = rawContentServices;
            _maps = maps;
        }

        public override void RegisterActions(ViewNode node)
        {
            base.RegisterActions(node);
            RegisterNavigationAction(node);
        }

        public override View Render(ViewNode node)
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
            // var columnMapOrder = content?.FirstOrDefault()?.Keys.Take(3).ToList();
            var detectectedColumns = content.FirstOrDefault().Select(kv => kv.Key).ToList();
            var columnMapOrder = tablebuffer.Columns?.Select(col => col.Name).ToList() ?? new List<string>();
            if(columnMapOrder == null || columnMapOrder.Count == 0)
            {
                columnMapOrder = detectectedColumns;
            }
            var headerContent = tablebuffer.Columns != null && tablebuffer.Columns.Count > 0 ?
                new List<IDictionary<string, object>>(){tablebuffer.Columns?.ToDictionary(col => col.Name, col => (object)col.DisplayName)} :
                null;
            var columnWidths = tablebuffer.Columns.Select(column => column.Width).ToList();
            var tableData = new TableData(content,headerContent,columnMapOrder, columnWidths);
            var view = new TableView(tableData);
            view.FocusedItemChanged = (line) =>
            {
                line.Keys.ToList().ForEach(key =>
                {
                    node.Parameters[$"line.{key}"] = line[key]?.ToString() ?? "";
                });
            };
            view.TriggerFocusedLineEvent();
            node.View = view;
            view.CanFocus = node.Buffer.Focusable;
            return view;
        }

        protected virtual void RegisterNavigationAction(ViewNode node)
        {
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.lineUp","/**",(_) => (node.View as TableView).FocusPrevLine()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/lineUp",$"{node.Scope}/**",(_) => (node.View as TableView).FocusPrevLine()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.lineDown","/**",(_) => (node.View as TableView).FocusNextLine()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/lineDown",$"{node.Scope}/**",(_) => (node.View as TableView).FocusNextLine()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.lineLast","/**",(_) => (node.View as TableView).FocusLastLine()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/lineLast",$"{node.Scope}/**",(_) => (node.View as TableView).FocusLastLine()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.lineFirst","/**",(_) => (node.View as TableView).FocusFirstLine()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/lineFirst",$"{node.Scope}/**",(_) => (node.View as TableView).FocusFirstLine()));
        }
    }
}