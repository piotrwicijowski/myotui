using System;
using System.Collections.Generic;

namespace myotui.Models
{
    public class ActionRegistration
    {
        public Guid Id {get; set;} = Guid.NewGuid();
        public Func<string,bool> Action {get; set;}
        public string Pattern {get; set;}
        public string ActionScope {get; set;}

        public static IList<ActionRegistration> RegistrationPair(string actionName, string nodeScope, Func<string,bool> action)
        {
            var result = new List<ActionRegistration>();
            result.Add(new ActionRegistration(){
                Pattern = $"{nodeScope}.{actionName}",
                ActionScope = "/**",
                Action = action});
            result.Add(new ActionRegistration(){
                Pattern = $"/{actionName}",
                ActionScope = $"{nodeScope}/**",
                Action = action});
            return result;

        }
    }
}