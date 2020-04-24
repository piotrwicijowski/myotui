using System.Collections.Generic;

namespace myotui.Models.Config
{
    public interface IBinding
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<string> Triggers {get; set;}
        public IEnumerable<string> Actions {get; set;}
        public string Scope {get; set;}
        public string Mode {get; set;}

    }
}