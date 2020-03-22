using Terminal.Gui;

namespace myotui.Models
{
    public interface IContent
    {
        public string Name {get; set;}
        public string Value {get; set;}
        public SizeHint Width {get; set;}
        public SizeHint Height {get; set;}
    }
}