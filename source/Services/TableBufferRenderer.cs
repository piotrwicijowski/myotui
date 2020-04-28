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
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.search","/**",(_) => (node.View as TableView).FocusSearch()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/search",$"{node.Scope}/**",(_) => (node.View as TableView).FocusSearch()));

            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.searchAccept","/**",(_) => (node.View as TableView).SearchAccept()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/searchAccept",$"{node.Scope}/**",(_) => (node.View as TableView).SearchAccept()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.searchAbort","/**",(_) => (node.View as TableView).SearchAbort()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/searchAbort",$"{node.Scope}/**",(_) => (node.View as TableView).SearchAbort()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.searchHistoryPrev","/**",(_) => (node.View as TableView).SearchHistoryPrev()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/searchHistoryPrev",$"{node.Scope}/**",(_) => (node.View as TableView).SearchHistoryPrev()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.searchHistoryNext","/**",(_) => (node.View as TableView).SearchHistoryNext()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/searchHistoryNext",$"{node.Scope}/**",(_) => (node.View as TableView).SearchHistoryNext()));

            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focusPrevResult","/**",(_) => (node.View as TableView).FocusPrevSearch()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/focusPrevResult",$"{node.Scope}/**",(_) => (node.View as TableView).FocusPrevSearch()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focusNextResult","/**",(_) => (node.View as TableView).FocusNextSearch()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/focusNextResult",$"{node.Scope}/**",(_) => (node.View as TableView).FocusNextSearch()));
        }

        protected virtual void RegisterFilterAction(ViewNode node)
        {
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.filter","/**",(_) => (node.View as TableView).FocusFilter()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/filter",$"{node.Scope}/**",(_) => (node.View as TableView).FocusFilter()));

            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.filterAccept","/**",(_) => (node.View as TableView).FilterAccept()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/filterAccept",$"{node.Scope}/**",(_) => (node.View as TableView).FilterAccept()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.filterAbort","/**",(_) => (node.View as TableView).FilterAbort()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/filterAbort",$"{node.Scope}/**",(_) => (node.View as TableView).FilterAbort()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.filterHistoryPrev","/**",(_) => (node.View as TableView).FilterHistoryPrev()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/filterHistoryPrev",$"{node.Scope}/**",(_) => (node.View as TableView).FilterHistoryPrev()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.filterHistoryNext","/**",(_) => (node.View as TableView).FilterHistoryNext()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/filterHistoryNext",$"{node.Scope}/**",(_) => (node.View as TableView).FilterHistoryNext()));
        }
    }
}