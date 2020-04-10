using System.Collections.Generic;
using System.Linq;
using myotui.Services;
using Terminal.Gui;

namespace myotui.Models
{
    using  KeyActionDictionary =  Dictionary<KeyList<Key>, List<(string action, string scope)>>;
    using  KeyPrefixToFullKeyListDictionary = Dictionary<KeyList<Key>,List<KeyList<Key>>>;
    public class KeyPrefixDictionary
    {
        private readonly KeyActionDictionary _globalKeyPrefixDictionary = new KeyActionDictionary();
        private readonly KeyPrefixToFullKeyListDictionary _keyPrefixToFullKeyListDictionary = new KeyPrefixToFullKeyListDictionary();

        protected readonly IScopeService _scopeService;

        public KeyPrefixDictionary(IScopeService scopeService)
        {
            _scopeService = scopeService;
        }

        public IList<string> GetAllActionsByKeyPrefix(IList<Key> keyPrefix, string scope)
        {
            var actionDictionaryInScope = GetKeyActionDictionaryByScope(scope);
            var allMatchingPrefixes = _keyPrefixToFullKeyListDictionary[new KeyList<Key>(keyPrefix)];
            if(allMatchingPrefixes == null || allMatchingPrefixes.Count == 0)
            {
                return null;
            }
            return allMatchingPrefixes.SelectMany(matchingPrefix => actionDictionaryInScope[matchingPrefix]).Select(item => item.action).ToList();
            
        }
        private KeyActionDictionary GetKeyActionDictionaryByScope(string curentScope)
        {
            var resultDictionary = _globalKeyPrefixDictionary.ToDictionary(
                keyValue => keyValue.Key,
                keyValue => keyValue.Value.Where(item =>
                    _scopeService.IsInScope(curentScope,item.scope)
                ).ToList()
            );
            var nullValuedKeys = resultDictionary.Where(pair => pair.Value == null || pair.Value.Count == 0).Select(pair => pair.Key);
            foreach(var nullvaluedKey in nullValuedKeys)
            {
                resultDictionary.Remove(nullvaluedKey);
            }
            return resultDictionary;
        }

        public void AddAction(IList<Key> keys, string action, string scope)
        {
            var fullKeyList = new KeyList<Key>(keys);
            if(!_globalKeyPrefixDictionary.ContainsKey(fullKeyList))
            {
                _globalKeyPrefixDictionary.Add(fullKeyList,new List<(string action, string scope)>());
            }
            _globalKeyPrefixDictionary[fullKeyList].Add((action,scope));

            for(int i = keys.Count - 1; i >= 0; --i)
            {
                var prefix = new KeyList<Key>(keys.SkipLast(i).ToList());
                if(!_keyPrefixToFullKeyListDictionary.ContainsKey(prefix))
                {
                    _keyPrefixToFullKeyListDictionary.Add(prefix,new List<KeyList<Key>>());
                }
                _keyPrefixToFullKeyListDictionary[prefix].Add(fullKeyList);
            }
        }
    }
}