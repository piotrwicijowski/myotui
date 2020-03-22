using System.Linq;
using myotui.Models;
using Terminal.Gui;

namespace myotui.Services
{
    public class HBladeBufferRenderer : IBufferRenderer
    {
        protected readonly IBufferService _bufferService;
        public HBladeBufferRenderer(IBufferService bufferService)
        {
            _bufferService = bufferService;
        }
        public View Render(IBuffer buffer)
        {
            var hboxbuffer = buffer as HBladeBuffer;
            var view = new View();
            var label = new Label(hboxbuffer.Name)
            {
                X = 0 + 1,
                Y = 0,
            };
            view.Add(label);
            var count = hboxbuffer.Content.Count(); 
            foreach (var element in hboxbuffer.Content.Select((value, i) => ( value, i )))
            {
                var elementLayout = _bufferService.RenderBuffer(element.value.Value);
                elementLayout.Y = 1;
                elementLayout.X = Pos.Percent(100/count * element.i);
                elementLayout.Height = Dim.Fill();
                elementLayout.Width = Dim.Percent(100/(count - element.i));

                view.Add(elementLayout);
            }
            return view;
        }
    }
}