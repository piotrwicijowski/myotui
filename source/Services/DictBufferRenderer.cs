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
    public class DictBufferRenderer : IBufferRenderer
    {
        protected readonly IActionService _actionService;
        protected readonly IKeyService _keyService;
        protected readonly IBufferService _bufferService;
        protected readonly IIndex<Type,IRawContentService> _rawContentServices;
        protected readonly IIndex<ValueMapType,IContentMapService> _maps;
        public DictBufferRenderer(IActionService actionService, IIndex<Type,IRawContentService> rawContentServices, IIndex<ValueMapType,IContentMapService> maps, IBufferService bufferService, IKeyService keyService)
        {
            _actionService = actionService;
            _rawContentServices = rawContentServices;
            _maps = maps;
            _bufferService = bufferService;
            _keyService = keyService;
        }

        public View Layout(ViewNode node)
        {
            return node.View;
        }

        public void RegisterEvents(ViewNode node)
        {
            RegisterFocusAction(node);
            RegisterCloseAction(node);
            RegisterNavigationAction(node);
        }

        public View Render(ViewNode node)
        {
            var buffer = node.Buffer;
            var scope = node.Scope;
            var dictBuffer = buffer as DictBuffer;
            var rawContentService = _rawContentServices[dictBuffer.Content.GetType()];
            var rawContent = rawContentService.GetRawOutput(dictBuffer.Content, node.Parameters);
            var map = _maps[dictBuffer.Content.Map];
            var dictContent = map.MapRawData(rawContent)?.FirstOrDefault();
            dictContent = dictContent ?? new Dictionary<string,object>();

            // var rowMapOrder = dictBuffer.Columns?.Select(col => col.Name).ToList() ?? new List<string>();
            var rowMapOrder = dictContent.Keys;
            var headerContent = new List<IDictionary<string, object>>(){
                new Dictionary<string,object>()
                {
                    {"key","Parameter Name"},{"value", "Value"}
                }
            };

            var columnMapOrder = new List<string>()
            {
                "key",
                "value"
            };
            var columnWidths = columnMapOrder.Select(x => 1.0).ToList();
            IList<IDictionary<string,object>> listContent = rowMapOrder.Select(rowName => new Dictionary<string,object>(){{"key", rowName},{"value", dictContent[rowName]}} as IDictionary<string,object>).ToList();
            var tableData = new TableData(listContent, headerContent, columnMapOrder, columnWidths);
            var view = new TableView(tableData);
            view.FocusedItemChanged = (line) =>
            {
                line?.Keys.ToList().ForEach(key =>
                {
                    node.Parameters[$"line.{key}"] = line[key]?.ToString() ?? "";
                });
            };
            view.TriggerFocusedLineEvent();
            // view.X = 0;
            // view.Y = 0;
            // view.Width = Dim.Fill();
            // view.Height = Dim.Fill();
            node.View = view;
            view.CanFocus = node.Buffer.Focusable;
            return view;
        }

        public void RegisterBindings(ViewNode node)
        {
            var bindings = node.Buffer.Bindings;

            bindings?
            .ToList()
            .ForEach(
                binding => binding
                    .Triggers
                    .Where(trigger => trigger.StartsWith("key "))
                    .SelectMany(
                        trigger => binding.Actions,
                        (trigger, action) => (trigger, action)
                    )
                    .ToList()
                    .ForEach(
                        pair => {
                            var (trigger, action) = pair;
                            _keyService.RegisterKeyActionTrigger(trigger, action, binding.Scope, node);
                        } 
                    )
                );
        }
        
        protected void RegisterFocusAction(ViewNode node)
        {
            _actionService.RegisterAction($"{node.Scope}.focus","/**",(_) => {node.Parent?.View.SetFocus(node.View);return true;});
        }
        
        protected virtual void RegisterCloseAction(ViewNode node)
        {
            _actionService.RegisterAction($"{node.Scope}.close","/**",(_) => _bufferService.CloseBuffer(node));
            _actionService.RegisterAction($"/close",$"{node.Scope}/**",(_) => _bufferService.CloseBuffer(node));
        }

        protected virtual void RegisterNavigationAction(ViewNode node)
        {
            _actionService.RegisterAction($"{node.Scope}.lineUp","/**",(_) => (node.View as TableView).FocusPrevLine());
            _actionService.RegisterAction($"/lineUp",$"{node.Scope}/**",(_) => (node.View as TableView).FocusPrevLine());
            _actionService.RegisterAction($"{node.Scope}.lineDown","/**",(_) => (node.View as TableView).FocusNextLine());
            _actionService.RegisterAction($"/lineDown",$"{node.Scope}/**",(_) => (node.View as TableView).FocusNextLine());
        }
    }
}