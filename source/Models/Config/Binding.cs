using System.Collections.Generic;
using System.ComponentModel;

namespace myotui.Models.Config
{
    public class Binding : IBinding
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<string> Triggers {get; set;}
        public IEnumerable<string> Actions {get; set;}

        [DefaultValue("./**")]
        public string Scope {get; set;} = "./**";
    }
}