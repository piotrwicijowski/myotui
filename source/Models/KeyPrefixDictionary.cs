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
        private readonly ISet<(KeyList<Key>,string,string)> _allMappingsSet = new HashSet<(KeyList<Key> keyList,string action,string scope)>();
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
            if(!_keyPrefixToFullKeyListDictionary.ContainsKey(new KeyList<Key>(keyPrefix)))
            {
                return null;
            }
            var allMatchingPrefixes = _keyPrefixToFullKeyListDictionary[new KeyList<Key>(keyPrefix)];
            if(allMatchingPrefixes == null || allMatchingPrefixes.Count == 0)
            {
                return null;
            }
            return allMatchingPrefixes.Where(matchingPrefix => actionDictionaryInScope.ContainsKey(matchingPrefix)).SelectMany(matchingPrefix => actionDictionaryInScope[matchingPrefix]).Select(item => item.action).ToList();
            
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
            _allMappingsSet.Add((fullKeyList,action,scope));
            RecalculateAction(fullKeyList, action, scope);
        }
        public void RecalculateAction(KeyList<Key> keys, string action, string scope)
        {
            if(!_globalKeyPrefixDictionary.ContainsKey(keys))
            {
                _globalKeyPrefixDictionary.Add(keys,new List<(string action, string scope)>());
            }
            _globalKeyPrefixDictionary[keys].Add((action,scope));

            for(int i = keys.Count - 1; i >= 0; --i)
            {
                var prefix = new KeyList<Key>(keys.SkipLast(i).ToList());
                if(!_keyPrefixToFullKeyListDictionary.ContainsKey(prefix))
                {
                    _keyPrefixToFullKeyListDictionary.Add(prefix,new List<KeyList<Key>>());
                }
                if(!_keyPrefixToFullKeyListDictionary[prefix].Contains(keys))
                {
                    _keyPrefixToFullKeyListDictionary[prefix].Add(keys);
                }
            }
        }

        public void RemoveAction(IList<Key> keys, string action, string scope)
        {
            var fullKeyList = new KeyList<Key>(keys);
            _allMappingsSet.Remove((fullKeyList,action,scope));
            _globalKeyPrefixDictionary.Clear();
            _keyPrefixToFullKeyListDictionary.Clear();
            foreach(var item in _allMappingsSet)
            {
                var (mappingKeys, mappingAction, mappingScope) = item;
                RecalculateAction(mappingKeys, mappingAction, mappingScope);
            }
        }
    }
}