using Terminal.Gui;

namespace myotui.Models
{
    public interface IContent
    {
        public string Name {get; set;}
        public View GetView();
    }
}