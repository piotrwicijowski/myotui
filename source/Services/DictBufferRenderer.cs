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
    public class DictBufferRenderer : ContentBufferRenderer
    {
        protected readonly IIndex<Type,IRawContentService> _rawContentServices;
        public DictBufferRenderer(IActionService actionService, IIndex<Type,IRawContentService> rawContentServices, IBufferService bufferService, IKeyService keyService, IModeService modeService) : base(actionService, keyService, bufferService, modeService)
        {
            _rawContentServices = rawContentServices;
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
            var dictBuffer = buffer as DictBuffer;
            var dictContent = node.Data as Dictionary<string,object>;
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
            // var columnWidths = columnMapOrder.Select(x => 1.0).ToList();
            var columnWidths = new List<SizeHint>()
            {
                new SizeHint(),
                new SizeHint(),
            };
            IList<IDictionary<string,object>> listContent = rowMapOrder.Select(rowName => new Dictionary<string,object>(){{"key", rowName},{"value", dictContent[rowName]}} as IDictionary<string,object>).ToList();
            var tableData = new TableData(listContent, headerContent, columnMapOrder, columnWidths);
            var view = new TableView(tableData, false, true);
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

        protected virtual void RegisterNavigationAction(ViewNode node)
        {

            node.RegisteredActions.AddRange((new List<ActionRegistration>(){
                }.Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "lineUp",
                        nodeScope : node.Scope,
                        action : (_) => (node.View as TableView).FocusPrevLine())
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "focusDown",
                        nodeScope : node.Scope,
                        action : (_) => (node.View as TableView).FocusNextLine())
                )
            ).Select(reg => _actionService.RegisterAction(reg)));
        }
    }
}