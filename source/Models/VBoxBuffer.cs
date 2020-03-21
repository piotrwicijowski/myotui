using System.Collections.Generic;
using Terminal.Gui;
using System.Linq;

namespace myotui.Models
{
    public class VBoxBuffer : IBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<IContent> Content {get; set;}
        public View BuildLayout()
        {
            var view = new View();
            var label = new Label(Name)
            {
                X = 0 + 1,
                Y = 0,
            };
            view.Add(label);
            var count = Content.Count(); 
            foreach (var element in Content.Select((value, i) => ( value, i )))
            {
                var elementLayout = element.value.GetView();
                elementLayout.X = 0;
                elementLayout.Y = Pos.Percent(100/count * element.i);
                elementLayout.Width = Dim.Fill();
                elementLayout.Height = Dim.Percent(100/count);

                view.Add(elementLayout);
            }
            return view;
        }
    }
}