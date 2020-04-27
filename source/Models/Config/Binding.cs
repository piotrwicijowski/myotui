using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace myotui.Models.Config
{
    public class Binding : IBinding
    {
        public string Name {get; set;}
        public string Description {get; set;}

        public virtual string Trigger {
            get => Triggers?.FirstOrDefault();
            set 
            {
                Triggers = new List<string>()
                {
                    value
                };
            }
        }
        public IEnumerable<string> Triggers {get; set;}
        public virtual string Action {
            get => Actions?.FirstOrDefault();
            set 
            {
                Actions = new List<string>()
                {
                    value
                };
            }
        }
        public IEnumerable<string> Actions {get; set;}

        [DefaultValue("./**")]
        public string Scope {get; set;} = "./**";
        public string Mode {get; set;}
    }
}