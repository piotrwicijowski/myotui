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
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focus","/**",(_) => {node.Parent?.View.SetFocus(node.View);return true;}));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focusRight","/**",(_) => _bufferService.FocusNextChild(node)));
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focusLeft","/**",(_) => _bufferService.FocusPreviousChild(node)));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/focusRight",$"{node.Scope}/**",(_) => _bufferService.FocusNextChild(node)));
            node.RegisteredActions.Add(_actionService.RegisterAction($"/focusLeft",$"{node.Scope}/**",(_) => _bufferService.FocusPreviousChild(node)));
        }
    }
}