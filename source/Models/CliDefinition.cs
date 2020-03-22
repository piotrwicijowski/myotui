using System.Collections.Generic;
using System.ComponentModel;

namespace myotui.Models
{
    public class CliDefinition
    {
        public string Input {get; set;}
        [DefaultValue(CliMapType.string_to_string_array)]
        public CliMapType Map {get; set;} = CliMapType.string_to_string_array;
        public IEnumerable<string> Select {get; set;}
        public IEnumerable<string> Order {get; set;}
    }
}