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
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.lineUp","/**",(_) => (node.View as TableView).FocusPrevLine()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/lineUp",$"{node.Scope}/**",(_) => (node.View as TableView).FocusPrevLine()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.lineDown","/**",(_) => (node.View as TableView).FocusNextLine()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/lineDown",$"{node.Scope}/**",(_) => (node.View as TableView).FocusNextLine()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.lineLast","/**",(_) => (node.View as TableView).FocusLastLine()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/lineLast",$"{node.Scope}/**",(_) => (node.View as TableView).FocusLastLine()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.lineFirst","/**",(_) => (node.View as TableView).FocusFirstLine()));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/lineFirst",$"{node.Scope}/**",(_) => (node.View as TableView).FocusFirstLine()));
        }

        protected virtual void RegisterRunActionAction(ViewNode node)
        {
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.runAction","/**",(_) => DispatchCurrentAction(node)));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/runAction",$"{node.Scope}/**",(_) => DispatchCurrentAction(node)));
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
    }
}