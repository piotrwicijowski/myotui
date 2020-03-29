using System.Collections.Generic;
using System.Linq;
using myotui.Models;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Services
{
    public class VBoxBufferRenderer : BufferRenderer
    {
        public VBoxBufferRenderer(IActionService actionService, IKeyService keyService, IBufferService bufferService) : base(actionService, keyService, bufferService)
        {
        }
        public override View Layout(ViewNode node)
        {
            var view = node.View;
            view.RemoveAll();
            var count = node.Children.Count();
            var dims = GetDims(node.Children.Select(child => child.Height));
            var elements = node.Children
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
                    element.Y = last == null ? 1 : Pos.Bottom(last);
                    view.Add(element);
                    return element;
                }
                );
            return view;
        }

        protected override void RegisterFocusAction(ViewNode node)
        {
            _actionService.RegisterAction($"{node.Scope}.focus","/**",() => {node.Parent?.View.SetFocus(node.View);return true;});
            _actionService.RegisterAction($"{node.Scope}.focusDown","/**",() => node.FocusNextChild());
            _actionService.RegisterAction($"{node.Scope}.focusUp","/**",() => node.FocusPreviousChild());
            _actionService.RegisterAction($"/focusDown",$"{node.Scope}/**",() => node.FocusNextChild());
            _actionService.RegisterAction($"/focusUp",$"{node.Scope}/**",() => node.FocusPreviousChild());
        }
    }
}