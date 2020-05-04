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
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("lineUp", node.Scope, (_) => (node.View as TableView).FocusPrevLine()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("lineDown", node.Scope, (_) => (node.View as TableView).FocusNextLine()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("lineLast", node.Scope, (_) => (node.View as TableView).FocusLastLine()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("lineFirst", node.Scope, (_) => (node.View as TableView).FocusFirstLine()));
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
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("search", node.Scope, (_) => (node.View as TableView).FocusSearch()));

            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("searchAccept", node.Scope, (_) => (node.View as TableView).SearchAccept()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("searchAbort", node.Scope, (_) => (node.View as TableView).SearchAbort()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("searchHistoryPrev", node.Scope, (_) => (node.View as TableView).SearchHistoryPrev()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("searchHistoryNext", node.Scope, (_) => (node.View as TableView).SearchHistoryNext()));

            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("focusPrevResult", node.Scope, (_) => (node.View as TableView).FocusPrevSearch()));
            node.RegisteredActions.AddRange(_actionService.RegisterActionPair("focusNextResult", node.Scope, (_) => (node.View as TableView).FocusNextSearch()));
        }
    }
}