namespace myotui.Models
{
    public class CliContent : IContent
    {
        public string Name {get; set;}
        public CliDefinition Cli {get; set;}
        public string GetValue()
        {
            return Cli.Input;
        }
    }
}