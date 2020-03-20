namespace myotui.Models
{
    public class Window : IWindow
    {
        public string Name {get; set;}
        public IContent Content {get; set;}
    }
}