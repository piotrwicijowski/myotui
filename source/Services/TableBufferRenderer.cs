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
        protected readonly IIndex<ValueMapType,IContentMapService> _maps;
        public TableBufferRenderer(IActionService actionService, IIndex<ValueMapType,IContentMapService> maps, IBufferService bufferService, IKeyService keyService) : base(actionService, keyService, bufferService)
        {
            _maps = maps;
        }

        public override void RegisterActions(ViewNode node)
        {
            base.RegisterActions(node);
            RegisterNavigationAction(node);
            var tablebuffer = node.Buffer as TableBuffer;
            if(tablebuffer != null && tablebuffer.HasSearch)
            {
                var tableView = (node.View as TableView);
                if(tableView != null)
                {
                    tableView.SearchOnEnter += (sender, args) =>
                    {
                        node.SkipKeyHandling = true;
                    };
                    tableView.SearchOnLeave += (sender, args) =>
                    {
                        node.SkipKeyHandling = false;
                    };
                }
                RegisterSearchAction(node);
            }
        }

        public override View Render(ViewNode node)
        {
            var buffer = node.Buffer;
            var tablebuffer = buffer as TableBuffer;
            var scope = node.Scope;
            var map = _maps[buffer.Content.Map];
            var content = map.MapRawData(node.Data) as List<IDictionary<string,object>>;

            content = content ?? new List<IDictionary<string,object>>();
            var detectectedColumns = content.FirstOrDefault().Select(kv => kv.Key).ToList();
            var columnMapOrder = tablebuffer.Columns?.Select(col => col.Name).ToList() ?? new List<string>();
            if(columnMapOrder == null || columnMapOrder.Count == 0)
            {
                columnMapOrder = detectectedColumns;
            }
            var headerContentExists = tablebuffer.Columns != null
                && tablebuffer.Columns.Count > 0
                && tablebuffer.Columns.Select(column => column.DisplayName).Any(displayName => !string.IsNullOrEmpty(displayName));
            var headerContent = headerContentExists
                ? new List<IDictionary<string, object>>(){tablebuffer.Columns?.ToDictionary(col => col.Name, col => (object)col.DisplayName)}
                : null;
            var columnWidths = tablebuffer.Columns.Select(column => column.Width).ToList();
            var tableData = new TableData(content,headerContent,columnMapOrder, columnWidths);
            var view = new TableView(tableData, tablebuffer.HasHeader, tablebuffer.HasSearch);
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

        protected virtual void RegisterSearchAction(ViewNode node)
        {
            // node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.search","/**",(_) => {node.SkipKeyHandling = true; return (node.View as TableView).FocusSearch();}));
            // node.RegisteredActions.Add(_actionService.RegisterAction($"/search",$"{node.Scope}/**",(_) => {node.SkipKeyHandling = true; return (node.View as TableView).FocusSearch();}));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.search","/**",(_) => (node.View as TableView).FocusSearch()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/search",$"{node.Scope}/**",(_) => (node.View as TableView).FocusSearch()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focusPrevResult","/**",(_) => (node.View as TableView).FocusPrevSearch()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/focusPrevResult",$"{node.Scope}/**",(_) => (node.View as TableView).FocusPrevSearch()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focusNextResult","/**",(_) => (node.View as TableView).FocusNextSearch()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/focusNextResult",$"{node.Scope}/**",(_) => (node.View as TableView).FocusNextSearch()));
        }
    }
}