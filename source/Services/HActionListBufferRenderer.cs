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
    public class HActionListBufferRenderer : ContentBufferRenderer
    {
        protected readonly IParameterService _parameterService;
        public HActionListBufferRenderer(IActionService actionService, IBufferService bufferService, IKeyService keyService, IModeService modeService, IParameterService parameterService) : base(actionService, keyService, bufferService, modeService)
        {
            _parameterService = parameterService;
        }

        public override void RegisterActions(ViewNode node)
        {
            base.RegisterActions(node);
            RegisterNavigationAction(node);
            RegisterRunActionAction(node);
            var hactionlistbuffer = node.Buffer as HActionListBuffer;
            if(hactionlistbuffer != null && hactionlistbuffer.HasSearch)
            {
                var tableView = (node.View as TableView);
                RegisterSearchAction(node);
            }

        }

        public override View Render(ViewNode node)
        {
            var buffer = node.Buffer;
            var hactionlistbuffer = buffer as HActionListBuffer;
            var scope = node.Scope;
            var content = node.Data as List<IDictionary<string,object>>;
            content = content ?? new List<IDictionary<string,object>>();

            var view = new HorizontalListView(content);
            view.FocusedItemChanged = (line) =>
            {
                line.Keys.ToList().ForEach(key =>
                {
                    node.Parameters[$"line.{key}"] = line[key]?.ToString() ?? "";
                });
            };
            view.TriggerFocusedColumnEvent();
            node.View = view;
            view.CanFocus = node.Buffer.Focusable;
            return view;
        }

        protected virtual void RegisterNavigationAction(ViewNode node)
        {
            var hListView = node.View as HorizontalListView;

            node.RegisteredActions.AddRange((new List<ActionRegistration>(){
                }.Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "columnLeft",
                        nodeScope : node.Scope,
                        action : (_) => hListView.FocusPrevColumn()
                    )
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "columnRight",
                        nodeScope : node.Scope,
                        action : (_) => hListView.FocusNextColumn()
                    )
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "columnFirst",
                        nodeScope : node.Scope,
                        action : (_) => hListView.FocusFirstColumn()
                    )
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "columnLast",
                        nodeScope : node.Scope,
                        action : (_) => hListView.FocusLastColumn()
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
            // node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.search","/**",(_) => (node.View as TableView).FocusSearch()));
            // node.RegisteredActions.Add(_actionService.RegisterAction($"/search",$"{node.Scope}/**",(_) => (node.View as TableView).FocusSearch()));

            // node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.searchAccept","/**",(_) => (node.View as TableView).SearchAccept()));
            // node.RegisteredActions.Add(_actionService.RegisterAction($"/searchAccept",$"{node.Scope}/**",(_) => (node.View as TableView).SearchAccept()));
            // node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.searchAbort","/**",(_) => (node.View as TableView).SearchAbort()));
            // node.RegisteredActions.Add(_actionService.RegisterAction($"/searchAbort",$"{node.Scope}/**",(_) => (node.View as TableView).SearchAbort()));
            // node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.searchHistoryPrev","/**",(_) => (node.View as TableView).SearchHistoryPrev()));
            // node.RegisteredActions.Add(_actionService.RegisterAction($"/searchHistoryPrev",$"{node.Scope}/**",(_) => (node.View as TableView).SearchHistoryPrev()));
            // node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.searchHistoryNext","/**",(_) => (node.View as TableView).SearchHistoryNext()));
            // node.RegisteredActions.Add(_actionService.RegisterAction($"/searchHistoryNext",$"{node.Scope}/**",(_) => (node.View as TableView).SearchHistoryNext()));

            // node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focusPrevResult","/**",(_) => (node.View as TableView).FocusPrevSearch()));
            // node.RegisteredActions.Add(_actionService.RegisterAction($"/focusPrevResult",$"{node.Scope}/**",(_) => (node.View as TableView).FocusPrevSearch()));
            // node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focusNextResult","/**",(_) => (node.View as TableView).FocusNextSearch()));
            // node.RegisteredActions.Add(_actionService.RegisterAction($"/focusNextResult",$"{node.Scope}/**",(_) => (node.View as TableView).FocusNextSearch()));
        }
    }
}