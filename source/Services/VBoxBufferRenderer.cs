using System.Linq;
using myotui.Models;
using Terminal.Gui;

namespace myotui.Services
{
    public class VBoxBufferRenderer : IBufferRenderer
    {
        protected readonly IBufferService _bufferService;
        public VBoxBufferRenderer(IBufferService bufferService)
        {
            _bufferService = bufferService;
        }
        public View Render(IBuffer buffer)
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
            foreach (var element in vboxbuffer.Content.Select((value, i) => ( value, i )))
            {
                var elementLayout = _bufferService.RenderBuffer(element.value.Value);
                elementLayout.Y = 1 + Pos.Percent(100/count * element.i);
                elementLayout.X = 0;
                elementLayout.Height = Dim.Percent(100/count) - 1;
                elementLayout.Width = Dim.Fill();

                view.Add(elementLayout);
            }
            return view;
        }
    }
}