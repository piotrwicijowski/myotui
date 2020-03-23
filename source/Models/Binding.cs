using System.Collections.Generic;

namespace myotui.Models
{
    public class Binding : IBinding
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<string> Triggers {get; set;}
        public IEnumerable<string> Actions {get; set;}
    }
}