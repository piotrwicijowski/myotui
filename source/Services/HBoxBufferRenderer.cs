using System.Linq;
using myotui.Models;
using Terminal.Gui;

namespace myotui.Services
{
    public class HBoxBufferRenderer : IBufferRenderer
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
                elementLayout.Y = 1;
                elementLayout.X = Pos.Percent(100/count * element.i);
                elementLayout.Height = Dim.Fill() - 1;
                elementLayout.Width = Dim.Percent(100/count);

                view.Add(elementLayout);
            }
            return view;
        }
    }
}