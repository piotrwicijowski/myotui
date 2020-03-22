using Terminal.Gui;

namespace myotui.Models
{
    public class BufferContent : IContent
    {
        public string Name {get; set;}
        public string Value {get; set;}
        public SizeHint Width {get; set;} = new SizeHint();
        public SizeHint Height {get; set;} = new SizeHint();
    }
}