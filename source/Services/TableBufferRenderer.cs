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
            var tableView = node.View as TableView;

            node.RegisteredActions.AddRange((new List<ActionRegistration>(){
                }.Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "lineUp",
                        nodeScope : node.Scope,
                        action : (_) => tableView.FocusPrevLine()
                    )
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "lineDown",
                        nodeScope : node.Scope,
                        action : (_) => tableView.FocusNextLine()
                    )
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "lineFirst",
                        nodeScope : node.Scope,
                        action : (_) => tableView.FocusFirstLine()
                    )
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "lineLast",
                        nodeScope : node.Scope,
                        action : (_) => tableView.FocusLastLine()
                    )
                )
            ).Select(reg => _actionService.RegisterAction(reg)));
        }

        protected virtual void RegisterSearchAction(ViewNode node)
        {
            var tableView = node.View as TableView;

            node.RegisteredActions.AddRange((new List<ActionRegistration>(){
                }.Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "search",
                        nodeScope : node.Scope,
                        action : (_) => tableView.FocusSearch()
                    )
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "searchAccept",
                        nodeScope : node.Scope,
                        action : (_) => tableView.SearchAccept()
                    )
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "searchAbort",
                        nodeScope : node.Scope,
                        action : (_) => tableView.SearchAbort()
                    )
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "searchHistoryPrev",
                        nodeScope : node.Scope,
                        action : (_) => tableView.SearchHistoryPrev()
                    )
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "searchHistoryNext",
                        nodeScope : node.Scope,
                        action : (_) => tableView.SearchHistoryNext()
                    )
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "focusPrevResult",
                        nodeScope : node.Scope,
                        action : (_) => tableView.FocusPrevSearch()
                    )
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "focusNextResult",
                        nodeScope : node.Scope,
                        action : (_) => tableView.FocusNextSearch()
                    )
                )
            ).Select(reg => _actionService.RegisterAction(reg)));
        }

        protected virtual void RegisterFilterAction(ViewNode node)
        {
            var tableView = node.View as TableView;

            node.RegisteredActions.AddRange((new List<ActionRegistration>(){
                }.Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "filter",
                        nodeScope : node.Scope,
                        action : (_) => tableView.FocusFilter()
                    )
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "filterAccept",
                        nodeScope : node.Scope,
                        action : (_) => tableView.FilterAccept()
                    )
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "filterAbort",
                        nodeScope : node.Scope,
                        action : (_) => tableView.FilterAbort()
                    )
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "filterHistoryPrev",
                        nodeScope : node.Scope,
                        action : (_) => tableView.FilterHistoryPrev()
                    )
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "filterHistoryNext",
                        nodeScope : node.Scope,
                        action : (_) => tableView.FilterHistoryNext()
                    )
                )
            ).Select(reg => _actionService.RegisterAction(reg)));
        }
    }
}