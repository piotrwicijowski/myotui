namespace myotui.Models
{
    public class Buffer : IBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public ILayout Layout {get; set;}
    }
}