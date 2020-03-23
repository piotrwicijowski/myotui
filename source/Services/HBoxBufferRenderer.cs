using System.Linq;
using myotui.Models;
using Terminal.Gui;

namespace myotui.Services
{
    public class HBoxBufferRenderer : BufferRenderer
    {
        protected readonly IBufferService _bufferService;

        public HBoxBufferRenderer(IBufferService bufferService, IActionService actionService) : base(actionService)
        {
            _bufferService = bufferService;
        }
        public override View Render(IBuffer buffer, string scope)
        {
            var hboxbuffer = buffer as HBoxBuffer;
            var view = new View();
            var label = new Label(hboxbuffer.Name)
            {
                X = 0 + 1,
                Y = 0,
            };
            view.Add(label);


            var count = hboxbuffer.Windows.Count(); 
            var dims = GetDims(hboxbuffer.Windows.Select(content => content.Width));
            var elements = hboxbuffer.Windows
                .Select((content,i) => {
                    var element = _bufferService.RenderBuffer(content.Value,$"{scope}/{content.Name}");
                    RegisterFocusAction(view,element,$"{scope}/{content.Name}");
                    return element;
                })
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