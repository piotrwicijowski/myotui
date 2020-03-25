using System.Linq;
using myotui.Models;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Services
{
    public class HBladeBufferRenderer : BufferRenderer
    {
        public HBladeBufferRenderer(IActionService actionService, IKeyService keyService) : base(actionService, keyService)
        {
        }
        public override View Layout(ViewNode node)
        {
            var view = node.View;
            var count = node.Children.Count();
            var layoutBuffer = node.Buffer as HBladeBuffer;
            var dims = GetDims(layoutBuffer.Windows.Select(content => content.Width));
            var elements = node.Children
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