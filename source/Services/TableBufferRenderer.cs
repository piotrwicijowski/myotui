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
        public TableBufferRenderer(IActionService actionService, IBufferService bufferService, IKeyService keyService, IModeService modeService) : base(actionService, keyService, bufferService, modeService)
        {
        }

        public override void RegisterActions(ViewNode node)
        {
            base.RegisterActions(node);
            RegisterNavigationAction(node);
            var tablebuffer = node.Buffer as TableBuffer;
            if(tablebuffer != null && tablebuffer.HasSearch)
            {
                var tableView = (node.View as TableView);
                RegisterSearchAction(node);
                RegisterFilterAction(node);
            }
        }

        public override View Render(ViewNode node)
        {
            var buffer = node.Buffer;
            var tablebuffer = buffer as TableBuffer;
            var scope = node.Scope;
            var content = node.Data as List<IDictionary<string,object>>;

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
            TableView view;
            if(node.View == null)
            {
                view = new TableView(tableData, tablebuffer.HasHeader, tablebuffer.HasSearch);
                view.FocusedItemChanged = (line) =>
                {
                    line?.Keys?.ToList().ForEach(key =>
                    {
                        node.Parameters[$"line.{key}"] = line[key]?.ToString() ?? "";
                    });
                };
                node.View = view;
            }
            else
            {
                view = node.View as TableView;
                view.SetData(tableData);
            }
            view.TriggerFocusedLineEvent();
            view.CanFocus = node.Buffer.Focusable;
            return view;
        }

        protected virtual void RegisterNavigationAction(ViewNode node)
        {
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("lineUp", node.Scope, (_) => (node.View as TableView).FocusPrevLine()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("lineDown", node.Scope, (_) => (node.View as TableView).FocusNextLine()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("lineLast", node.Scope, (_) => (node.View as TableView).FocusLastLine()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("lineFirst", node.Scope, (_) => (node.View as TableView).FocusFirstLine()));
        }

        protected virtual void RegisterSearchAction(ViewNode node)
        {
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("search", node.Scope, (_) => (node.View as TableView).FocusSearch()));

            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("searchAccept", node.Scope, (_) => (node.View as TableView).SearchAccept()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("searchAbort", node.Scope, (_) => (node.View as TableView).SearchAbort()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("searchHistoryPrev", node.Scope, (_) => (node.View as TableView).SearchHistoryPrev()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("searchHistoryNext", node.Scope, (_) => (node.View as TableView).SearchHistoryNext()));

            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("focusPrevResult", node.Scope, (_) => (node.View as TableView).FocusPrevSearch()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("focusNextResult", node.Scope, (_) => (node.View as TableView).FocusNextSearch()));
        }

        protected virtual void RegisterFilterAction(ViewNode node)
        {
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("filter", node.Scope, (_) => (node.View as TableView).FocusFilter()));

            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("filterAccept", node.Scope, (_) => (node.View as TableView).FilterAccept()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("filterAbort", node.Scope, (_) => (node.View as TableView).FilterAbort()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("filterHistoryPrev", node.Scope, (_) => (node.View as TableView).FilterHistoryPrev()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("filterHistoryNext", node.Scope, (_) => (node.View as TableView).FilterHistoryNext()));

        }
    }
}