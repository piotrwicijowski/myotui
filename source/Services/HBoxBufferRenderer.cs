using System.Collections.Generic;
using System.Linq;
using myotui.Models;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Services
{
    public class HBoxBufferRenderer : LayoutBufferRenderer
    {
        public HBoxBufferRenderer(IActionService actionService, IKeyService keyService, IBufferService bufferService, IModeService modeService) : base(actionService, keyService, bufferService, modeService)
        {
        }

        public override View Layout(ViewNode node)
        {
            var view = node.View;
            view.RemoveAll();
            view.Clear();
            var dims = GetDims(node.ChildrenWithSplitters().Select(child => child.Width));
            var elements = node.ChildrenWithSplitters()
                .Select(child => child.View)
                .Zip(dims, (element, dim) => 
                {
                    element.Y = 0;
                    element.Height = Dim.Fill();
                    element.Width = dim;
                    return element;
                })
                .Aggregate<View, View>(null, (last, element) => 
                {
                    element.X = last == null ? 0 : Pos.Right(last);
                    view.Add(element);
                    return element;
                }
                );
            return view;
        }

        protected override void RegisterFocusAction(ViewNode node)
        {
            node.RegisteredActions.AddRange((new List<ActionRegistration>(){
                new ActionRegistration(){
                    Pattern = $"{node.Scope}.focus",
                    ActionScope = "/**",
                    Action = (_) => {node.Parent?.View.SetFocus(node.View);return true;}},
                }.Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "focusRight",
                        nodeScope : node.Scope,
                        action : (_) => _bufferService.FocusNextChild(node))
                ).Concat(
                    ActionRegistration.RegistrationPair(
                        actionName : "focusLeft",
                        nodeScope : node.Scope,
                        action : (_) => _bufferService.FocusPreviousChild(node))
                )
            ).Select(reg => _actionService.RegisterAction(reg)));
        }
    }
}