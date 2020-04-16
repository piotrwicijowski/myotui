using System.Linq;
using myotui.Models;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Services
{
    public class HBladeBufferRenderer : LayoutBufferRenderer
    {
        public HBladeBufferRenderer(IActionService actionService, IKeyService keyService, IBufferService bufferService) : base(actionService, keyService, bufferService)
        {
        }

        public override View Layout(ViewNode node)
        {
            var view = node.View;
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
    }
}