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
    public class VActionListBufferRenderer : ContentBufferRenderer
    {
        protected readonly IParameterService _parameterService;
        public VActionListBufferRenderer(IActionService actionService, IBufferService bufferService, IKeyService keyService, IModeService modeService, IParameterService parameterService) : base(actionService, keyService, bufferService, modeService)
        {
            _parameterService = parameterService;
        }

        public override void RegisterActions(ViewNode node)
        {
            base.RegisterActions(node);
            RegisterNavigationAction(node);
            RegisterRunActionAction(node);
            var vactionlistbuffer = node.Buffer as VActionListBuffer;
            if(vactionlistbuffer != null && vactionlistbuffer.HasSearch)
            {
                var tableView = (node.View as TableView);
                RegisterSearchAction(node);
            }

        }

        public override View Render(ViewNode node)
        {
            var buffer = node.Buffer;
            var vactionlistbuffer = buffer as VActionListBuffer;
            var scope = node.Scope;
            var content = node.Data as List<IDictionary<string,object>>;
            content = content ?? new List<IDictionary<string,object>>();
            var columnMapOrder = new List<string>(){"DisplayName"};
            // var columnWidths = tablebuffer.Columns.Select(column => column.Width).ToList();
            var columnWidths = new List<SizeHint>(){
                new SizeHint()
                {
                    Mode = SizeMode.Fill,
                    FillRatio = 1.0,
                }
            };
            var tableData = new TableData(content,headerContent: null,columnMapOrder, columnWidths);
            var view = new TableView(tableData, hasHeader: false, vactionlistbuffer.HasSearch);
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

        protected virtual void RegisterRunActionAction(ViewNode node)
        {
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("runAction", node.Scope, (_) => DispatchCurrentAction(node)));
        }

        protected bool DispatchCurrentAction(ViewNode node)
        {
            if(!node.Parameters.ContainsKey("line.Action"))
            {
                return false;
            }
            var currentActionString = node.Parameters["line.Action"];
            var substitutedAction = _parameterService.SubstituteParameters(currentActionString, node.Parameters);
            _actionService.DispatchAction(substitutedAction, node.Scope);
            return true;
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
    }
}