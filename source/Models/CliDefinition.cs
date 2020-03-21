using System.Collections.Generic;

namespace myotui.Models
{
    public class CliDefinition
    {
        public string Input {get; set;}
        public CliMapType Map {get; set;} 
        public IEnumerable<string> Select {get; set;}
        public IEnumerable<string> Order {get; set;}
    }
}