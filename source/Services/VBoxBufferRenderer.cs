using System.Collections.Generic;
using System.Linq;
using myotui.Models;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Services
{
    public class VBoxBufferRenderer : LayoutBufferRenderer
    {
        public VBoxBufferRenderer(IActionService actionService, IKeyService keyService, IBufferService bufferService, IModeService modeService) : base(actionService, keyService, bufferService, modeService)
        {
        }
        public override View Layout(ViewNode node)
        {
            var view = node.View;
            view.RemoveAll();
            view.Clear();
            var dims = GetDims(node.ChildrenWithSplitters().Select(child => child.Height));
            var elements = node.ChildrenWithSplitters()
                .Select(child => child.View)
                .Zip(dims, (element, dim) => 
                {
                    element.X = 0;
                    element.Height = dim;
                    element.Width = Dim.Fill();
                    return element;
                })
                .Aggregate<View, View>(null, (last, element) => 
                {
                    element.Y = last == null ? 0 : Pos.Bottom(last);
                    view.Add(element);
                    return element;
                }
                );
            return view;
        }

        protected override void RegisterFocusAction(ViewNode node)
        {
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focus","/**",(_) => {node.Parent?.View.SetFocus(node.View);return true;}));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focusDown","/**",(_) => _bufferService.FocusNextChild(node)));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focusUp","/**",(_) => _bufferService.FocusPreviousChild(node)));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/focusDown",$"{node.Scope}/**",(_) => _bufferService.FocusNextChild(node)));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/focusUp",$"{node.Scope}/**",(_) => _bufferService.FocusPreviousChild(node)));
        }
    }
}