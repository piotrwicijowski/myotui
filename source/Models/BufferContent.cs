using Terminal.Gui;

namespace myotui.Models
{
    public class BufferContent : IContent
    {
        public string Name {get; set;}
        public string Value {get; set;}
        public View GetView()
        {
            return new View()
            {

            };
        }
        
    }
}