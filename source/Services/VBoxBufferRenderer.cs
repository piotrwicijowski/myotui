using System.Linq;
using myotui.Models;
using Terminal.Gui;

namespace myotui.Services
{
    public class VBoxBufferRenderer : IBufferRenderer
    {
        public View Render(IBuffer buffer)
        {
            var view = new View();
            var label = new Label(buffer.Name)
            {
                X = 0 + 1,
                Y = 0,
            };
            view.Add(label);
            var count = buffer.Content.Count(); 
            foreach (var element in buffer.Content.Select((value, i) => ( value, i )))
            {
                var elementLayout = element.value.GetView();
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