using System.Linq;
using myotui.Models;
using Terminal.Gui;

namespace myotui.Services
{
    public class HBoxBufferRenderer : BufferRenderer
    {
        protected readonly IBufferService _bufferService;
        public HBoxBufferRenderer(IBufferService bufferService)
        {
            _bufferService = bufferService;
        }
        public override View Render(IBuffer buffer)
        {
            var hboxbuffer = buffer as HBoxBuffer;
            var view = new View();
            var label = new Label(hboxbuffer.Name)
            {
                X = 0 + 1,
                Y = 0,
            };
            view.Add(label);


            var count = hboxbuffer.Content.Count(); 
            var dims = GetDims(hboxbuffer.Content.Select(content => content.Width));
            var elements = hboxbuffer.Content
                .Select((content,i) => _bufferService.RenderBuffer(content.Value))
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

            
            // var count = hboxbuffer.Content.Count(); 
            // foreach (var element in hboxbuffer.Content.Select((value, i) => ( value, i )))
            // {
            //     var elementLayout = _bufferService.RenderBuffer(element.value.Value);
            //     elementLayout.Y = 1;
            //     elementLayout.X = Pos.Percent(100/count * element.i);
            //     elementLayout.Height = Dim.Fill();
            //     elementLayout.Width = Dim.Percent(100/(count - element.i));

            //     view.Add(elementLayout);
            // }
            // return view;
        }
    }
}