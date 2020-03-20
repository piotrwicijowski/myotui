using System.Collections.Generic;

namespace myotui.Models
{
    public class App : IApp
    {
        public string Name {get; set;}

        public IEnumerable<IModeDefinition> Modes {get; set;}
    }
}