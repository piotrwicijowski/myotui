using System.Linq;
using myotui.Models;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Services
{
    public class HBoxBufferRenderer : BufferRenderer
    {
        public HBoxBufferRenderer(IActionService actionService, IKeyService keyService) : base(actionService, keyService)
        {
        }

        public override View Layout(ViewNode node)
        {
            var view = node.View;
            var count = node.Children.Count();
            var layoutBuffer = node.Buffer as HBoxBuffer;
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
        // public override View Render(IBuffer buffer, string scope)
        // {
        //     var hboxbuffer = buffer as HBoxBuffer;
        //     var view = new View();
        //     var label = new Label(hboxbuffer.Name)
        //     {
        //         X = 0 + 1,
        //         Y = 0,
        //     };
        //     view.Add(label);


        //     var count = hboxbuffer.Windows.Count(); 
        //     var dims = GetDims(hboxbuffer.Windows.Select(content => content.Width));
        //     var elements = hboxbuffer.Windows
        //         .Select((content,i) => {
        //             var element = _bufferService.RenderBuffer(content.Value,$"{scope}/{content.Name}");
        //             RegisterFocusAction(view,element,$"{scope}/{content.Name}");
        //             return element;
        //         })
        //         .Zip(dims, (element, dim) => 
        //         {
        //             element.Y = 0;
        //             element.Height = Dim.Fill();
        //             element.Width = dim;
        //             return element;
        //         })
        //         .Aggregate<View, View>(null, (last, element) => 
        //         {
        //             element.X = last == null ? 0 : Pos.Right(last);
        //             view.Add(element);
        //             return element;
        //         }
        //         );
        //     RegisterBindings(hboxbuffer.Bindings, scope);
        //     return view;
        // }
    }
}