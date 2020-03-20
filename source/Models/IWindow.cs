namespace myotui.Models
{
    public interface IWindow
    {
        public string Name {get; set;}
        public IContent Content {get; set;}
    }
}