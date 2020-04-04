using System;
using System.Collections.Generic;
using System.Linq;
using myotui.Models;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Services
{
    public class BufferRenderer : IBufferRenderer
    {
        protected readonly IActionService _actionService;
        protected readonly IBufferService _bufferService;
        protected readonly IKeyService _keyService;
        protected BufferRenderer(IActionService actionService, IKeyService keyService, IBufferService bufferService)
        {
            _actionService = actionService;
            _keyService = keyService;
            _bufferService = bufferService;
        }
        public virtual View Render(ViewNode node)
        {
            var view = new View();
            var label = new Label(node.Buffer.Name)
            {
                X = 0 + 1,
                Y = 0,
            };
            view.Add(label);
            node.View = view;
            return view;
        }

        public void RegisterEvents(ViewNode node)
        {
            RegisterFocusAction(node);
            RegisterOpenAction(node);
        }

        public virtual View Layout(ViewNode node)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Dim> GetDims(IEnumerable<SizeHint> sizeHints)
        {
            var dims = new List<Dim>();
            var ratioScale = 100.0/sizeHints.Where(hint => hint.Mode == SizeMode.Fill).Sum(hint => hint.FillRatio);
            sizeHints.Aggregate(100.0, (remainingPercentage, hint) => 
            {
                var fillPercentage = hint.Mode == SizeMode.Fill ? Clamp(value: hint.FillRatio * ratioScale, min: hint.FillMinPercentage, max: hint.FillMaxPercentage) : 0.0;
                Dim dim = hint.Mode switch
                {
                    SizeMode.Fixed => Dim.Sized(hint.Fixed),
                    SizeMode.Fill => Dim.Percent((float)(fillPercentage/remainingPercentage*100.0)),
                };
                dims.Add(dim);
                return remainingPercentage -  fillPercentage;

            });
            return dims;
        }

        protected virtual void RegisterFocusAction(ViewNode node)
        {
            _actionService.RegisterAction($"{node.Scope}.focus","/**",(_) => {node.Parent?.View.SetFocus(node.View);return true;});
            _actionService.RegisterAction($"{node.Scope}.focusNext",node.Scope,(_) => node.FocusNextChild());
            _actionService.RegisterAction($"{node.Scope}.focusPrev",node.Scope,(_) => node.FocusPreviousChild());
            _actionService.RegisterAction($"/focusNext",$"{node.Scope}/**",(_) => node.FocusNextChild());
            _actionService.RegisterAction($"/focusPrev",$"{node.Scope}/**",(_) => node.FocusPreviousChild());
        }

        protected virtual void RegisterOpenAction(ViewNode node)
        {
            _actionService.RegisterAction($"{node.Scope}.open","/**",(parameters) => {

                var parametersSplit = parameters.Split(" ");
                var bufferName = parametersSplit.FirstOrDefault();
                var bufferParameters = parametersSplit.Length <= 1 ? "" : string.Join(' ',parametersSplit.Skip(1));
                _bufferService.OpenNewBuffer(node, bufferName, bufferParameters);
                return true;
            });
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

        private static double Clamp( double value, double min, double max )
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}