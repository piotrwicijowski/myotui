using System.Collections.Generic;

namespace myotui.Models
{
    public class App
    {
        public string Name {get; set;}

        public IEnumerable<ModeDefinition> Modes {get; set;}
    }
}