using System.Collections.Generic;
namespace myotui.Models
{
    public class TableBuffer : IBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public CliDefinition Cli {get; set;}
    }
}