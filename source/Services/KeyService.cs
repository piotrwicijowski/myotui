using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Terminal.Gui;

namespace myotui.Services {
    public class KeyService : IKeyService {
        //TODO this is only for demonstration purposes
        protected readonly IActionService _actionService;
        protected readonly Dictionary<Key, (string action, string scope)> _triggerActionDictionary = new Dictionary<Key, (string action, string scope)>();
        public const string triggerPattern = "key ";
        public KeyService (IActionService actionService)
        {
            _actionService = actionService;
        }
        public void ProcessKeyEvent (KeyEvent keyEvent)
        {

            var isRegistered = _triggerActionDictionary.TryGetValue(keyEvent.Key, out var item);
            if(isRegistered)
            {
                var (action, scope) = item;
                _actionService.DispatchAction(action,scope);
            }
            // switch(keyEvent.Key)
            // {
            //     case (Key)('h'):
            //         break;
            //     case (Key)('l'):
            //         _actionService.DispatchAction("/body/main.focus","/");
            //         break;
            // }

        }

        public void RegisterKeyActionTrigger(string trigger, string action, string scope)
        {
            var keys = DecodeKeySequence(trigger);
            if(keys.Any()){
                _triggerActionDictionary.Add(
                    keys.FirstOrDefault(), (action, scope)
                );
            }

        }

        private List<Key> DecodeKeySequence(string trigger)
        {
            var matches = Regex.Matches(
                    trigger.Substring(triggerPattern.Length),
                    @"(<([^<>]+)>)|([^<> ])"
                )
                .Select(
                    match => 
                    {
                        var value = match.Value;
                        var isSpecial = match.Groups[2].Success;
                        var isSpecialCorrect = Enum.TryParse<Key>(match.Groups[2].Value, out var specialKey);
                        return (isSpecial && isSpecialCorrect) ? specialKey : (Key)(value[0]);
                    }
                ).ToList();
            return matches;
        }
    }
}