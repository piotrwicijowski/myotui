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
        protected readonly IModeService _modeService;
        protected BufferRenderer(IActionService actionService, IKeyService keyService, IBufferService bufferService, IModeService modeService)
        {
            _actionService = actionService;
            _keyService = keyService;
            _bufferService = bufferService;
            _modeService = modeService;
        }

        public virtual View Render(ViewNode node)
        {
            var view = new View();
            node.View = view;
            view.CanFocus = node.Buffer.Focusable;
            return view;
        }

        public virtual void RegisterActions(ViewNode node)
        {
            RegisterCloseAction(node);
        }

        public virtual void RemoveActions(ViewNode node)
        {
            foreach(var actionGuid in node.RegisteredActions)
            {
                _actionService.RemoveAction(actionGuid);
            }
        }

        public virtual View Layout(ViewNode node)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Dim> GetDims(IEnumerable<SizeHint> sizeHints)
        {
            var dims = new List<Dim>();
            var ratioScale = 100.0/sizeHints.Where(hint => hint.Mode == SizeMode.Fill || hint.Mode == SizeMode.Auto).Sum(hint => hint.FillRatio);
            sizeHints.Aggregate(100.0, (remainingPercentage, hint) => 
            {
                var fillPercentage = 
                    (hint.Mode == SizeMode.Fill ||
                    hint.Mode == SizeMode.Auto)
                    ? Helpers.Clamp(value: hint.FillRatio * ratioScale, min: hint.FillMinPercentage, max: hint.FillMaxPercentage)
                    : 0.0;
                Dim dim = hint.Mode switch
                {
                    SizeMode.Fixed => Dim.Sized(hint.Fixed),
                    SizeMode.Fill => Dim.Percent((float)(fillPercentage/remainingPercentage*100.0)),
                    SizeMode.Auto => Dim.Percent((float)(fillPercentage/remainingPercentage*100.0)),
                };
                dims.Add(dim);
                return remainingPercentage -  fillPercentage;

            });
            return dims;
        }

        protected virtual void RegisterCloseAction(ViewNode node)
        {
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.close","/**",(_) => _bufferService.CloseBuffer(node)));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/close",$"{node.Scope}/**",(_) => _bufferService.CloseBuffer(node)));
        }

        private void HandleBindings(ViewNode node, Action<string,string,string,string,ViewNode> bindingAction)
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
                            var mode = binding.Mode ?? _modeService.DefaultMode;
                            bindingAction(trigger, action, binding.Scope, mode, node);
                        } 
                    )
                );

        }

        public void RegisterBindings(ViewNode node)
        {
            HandleBindings(node, _keyService.RegisterKeyActionTrigger);
        }

        public void RemoveBindings(ViewNode node)
        {
            HandleBindings(node, _keyService.RemoveKeyActionTrigger);
        }

    }
}