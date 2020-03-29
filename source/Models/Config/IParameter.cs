namespace myotui.Models.Config
{
    public interface IParameter
    {
        public string Name {get; set;}
        public string Type {get; set;}
        public string DefaultValue {get; set;}
    }
}