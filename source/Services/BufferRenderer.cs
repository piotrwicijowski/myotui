using System;
using System.Collections.Generic;
using System.Linq;
using myotui.Models;
using Terminal.Gui;

namespace myotui.Services
{
    public class BufferRenderer : IBufferRenderer
    {
        protected readonly IActionService _actionService;
        protected readonly IKeyService _keyService;
        protected BufferRenderer(IActionService actionService, IKeyService keyService)
        {
            _actionService = actionService;
            _keyService = keyService;
        }
        public virtual View Render(IBuffer buffer, string scope)
        {
            throw new System.NotImplementedException();
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

        protected void RegisterFocusAction(View parentView, View childView, string scope)
        {
            _actionService.RegisterAction($"{scope}.focus",scope,() => parentView.SetFocus(childView));
        }

        protected void RegisterBindings(IEnumerable<IBinding> bindings, string scope)
        {
            bindings?
            .ToList()
            .ForEach(
                binding => binding
                    .Triggers
                    .Where(trigger => trigger.StartsWith("key "))
                    // .SelectMany<string, string, object>(
                    //     trigger => binding.Actions,
                    //     (trigger, action) => {
                    //         _keyService.RegisterKeyActionTrigger(trigger, action, scope);
                    //         return null;
                    //     }
                    // )
                    .SelectMany(
                        trigger => binding.Actions,
                        (trigger, action) => (trigger, action)
                    )
                    .ToList()
                    .ForEach(
                        pair => {
                            var (trigger, action) = pair;
                            _keyService.RegisterKeyActionTrigger(trigger, action, scope);
                        } 
                    )
                );
            // var keyBindings = bindings.Where(binding => binding.Triggers);
            // _keyService.reg

            // _actionService.RegisterAction($"{scope}.focus",scope,() => parentView.SetFocus(childView));
        }


        public static double Clamp( double value, double min, double max )
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}