using System.Collections.Generic;
using System.Linq;
using myotui.Models;
using Terminal.Gui;

namespace myotui.Services
{
    public class VBoxBufferRenderer : BufferRenderer
    {
        protected readonly IBufferService _bufferService;
        public VBoxBufferRenderer(IBufferService bufferService)
        {
            _bufferService = bufferService;
        }
        public override View Render(IBuffer buffer)
        {
            var vboxbuffer = buffer as VBoxBuffer;
            var view = new View();
            var label = new Label(vboxbuffer.Name)
            {
                X = 0 + 1,
                Y = 0,
            };
            view.Add(label);
            var count = vboxbuffer.Content.Count(); 
            var dims = GetDims(vboxbuffer.Content.Select(content => content.Height));
            var elements = vboxbuffer.Content
                .Select((content,i) => _bufferService.RenderBuffer(content.Value))
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
    }
}