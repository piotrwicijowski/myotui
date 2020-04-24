using System.Collections.Generic;
using System.Linq;
using myotui.Services;
using Terminal.Gui;

namespace myotui.Models
{
    using  KeyActionDictionary =  Dictionary<KeyList<Key>, List<(string action, string scope, string mode)>>;
    using  KeyPrefixToFullKeyListDictionary = Dictionary<KeyList<Key>,List<KeyList<Key>>>;
    public class KeyPrefixDictionary
    {
        private readonly ISet<(KeyList<Key> keyList,string action,string scope, string mode)> _allMappingsSet = new HashSet<(KeyList<Key> keyList,string action,string scope, string mode)>();
        private readonly KeyActionDictionary _globalKeyPrefixDictionary = new KeyActionDictionary();
        private readonly KeyPrefixToFullKeyListDictionary _keyPrefixToFullKeyListDictionary = new KeyPrefixToFullKeyListDictionary();
        protected readonly IModeService _modeService;

        protected readonly IScopeService _scopeService;

        public KeyPrefixDictionary(IScopeService scopeService, IModeService modeService)
        {
            _scopeService = scopeService;
            _modeService = modeService;
        }

        public IList<string> GetAllActionsByKeyPrefix(IList<Key> keyPrefix, string scope, string mode = null)
        {
            if(mode == null)
            {
                mode = _modeService.CurrentMode;
            }
            var actionDictionaryInScope = GetKeyActionDictionaryByScope(scope, mode);
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
        private KeyActionDictionary GetKeyActionDictionaryByScope(string curentScope, string mode)
        {
            var resultDictionary = _globalKeyPrefixDictionary.ToDictionary(
                keyValue => keyValue.Key,
                keyValue => keyValue.Value.Where(item =>
                    _scopeService.IsInScope(curentScope,item.scope) && mode == item.mode
                ).ToList()
            );
            var nullValuedKeys = resultDictionary.Where(pair => pair.Value == null || pair.Value.Count == 0).Select(pair => pair.Key);
            foreach(var nullvaluedKey in nullValuedKeys)
            {
                resultDictionary.Remove(nullvaluedKey);
            }
            return resultDictionary;
        }

        public void AddAction(IList<Key> keys, string action, string scope, string mode)
        {
            var fullKeyList = new KeyList<Key>(keys);
            _allMappingsSet.Add((fullKeyList,action,scope, mode));
            RecalculateAction(fullKeyList, action, scope, mode);
        }
        public void RecalculateAction(KeyList<Key> keys, string action, string scope, string mode)
        {
            if(!_globalKeyPrefixDictionary.ContainsKey(keys))
            {
                _globalKeyPrefixDictionary.Add(keys,new List<(string action, string scope, string mode)>());
            }
            _globalKeyPrefixDictionary[keys].Add((action,scope,mode));

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

        public void RemoveAction(IList<Key> keys, string action, string scope, string mode)
        {
            var fullKeyList = new KeyList<Key>(keys);
            _allMappingsSet.Remove((fullKeyList,action,scope,mode));
            _globalKeyPrefixDictionary.Clear();
            _keyPrefixToFullKeyListDictionary.Clear();
            foreach(var item in _allMappingsSet)
            {
                var (mappingKeys, mappingAction, mappingScope, mappingMode) = item;
                RecalculateAction(mappingKeys, mappingAction, mappingScope, mappingMode);
            }
        }
    }
}