using System.Collections.Generic;
using System.Linq;
using myotui.Services;
using Terminal.Gui;

namespace myotui.Models
{
    using  KeyActionDictionary =  Dictionary<KeyList<Key>, List<(KeyList<string> actions, string scope, string mode)>>;
    using  KeyPrefixToFullKeyListDictionary = Dictionary<KeyList<Key>,List<KeyList<Key>>>;
    public class KeyPrefixDictionary
    {
        private readonly ISet<(KeyList<Key> keyList,KeyList<string> actions,string scope, string mode)> _allMappingsSet = new HashSet<(KeyList<Key> keyList,KeyList<string> actions,string scope, string mode)>();
        private readonly KeyActionDictionary _globalKeyPrefixDictionary = new KeyActionDictionary();
        private readonly KeyPrefixToFullKeyListDictionary _keyPrefixToFullKeyListDictionary = new KeyPrefixToFullKeyListDictionary();
        protected readonly IModeService _modeService;

        protected readonly IScopeService _scopeService;

        public KeyPrefixDictionary(IScopeService scopeService, IModeService modeService)
        {
            _scopeService = scopeService;
            _modeService = modeService;
        }

        public IList<List<string>> GetAllActionsByKeyPrefix(IList<Key> keyPrefix, string scope, string mode = null)
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
            return allMatchingPrefixes.Where(matchingPrefix => actionDictionaryInScope.ContainsKey(matchingPrefix)).SelectMany(matchingPrefix => actionDictionaryInScope[matchingPrefix]).Select(item => item.actions.ToList()).ToList();
            
        }
        private KeyActionDictionary GetKeyActionDictionaryByScope(string curentScope)
        {
            var resultDictionary = _globalKeyPrefixDictionary.ToDictionary(
                keyValue => keyValue.Key,
                keyValue => keyValue.Value.Where(item =>
                    _scopeService.IsInScope(curentScope,item.scope) && _modeService.BindingMatchesCurrentMode(item.mode)
                ).ToList()
            );
            var nullValuedKeys = resultDictionary.Where(pair => pair.Value == null || pair.Value.Count == 0).Select(pair => pair.Key);
            foreach(var nullvaluedKey in nullValuedKeys)
            {
                resultDictionary.Remove(nullvaluedKey);
            }
            return resultDictionary;
        }

        public void AddAction(IList<Key> keys, List<string> actions, string scope, string mode)
        {
            var fullKeyList = new KeyList<Key>(keys);
            var actionsKeyList = new KeyList<string>(actions);
            _allMappingsSet.Add((fullKeyList,actionsKeyList,scope, mode));
            RecalculateAction(fullKeyList, actionsKeyList, scope, mode);
        }
        public void RecalculateAction(KeyList<Key> keys, KeyList<string> actions, string scope, string mode)
        {
            if(!_globalKeyPrefixDictionary.ContainsKey(keys))
            {
                _globalKeyPrefixDictionary.Add(keys,new List<(KeyList<string> actions, string scope, string mode)>());
            }
            _globalKeyPrefixDictionary[keys].Add((actions,scope,mode));

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

        public void RemoveAction(IList<Key> keys, List<string> actions, string scope, string mode)
        {
            var fullKeyList = new KeyList<Key>(keys);
            _allMappingsSet.Remove((fullKeyList,new KeyList<string>(actions),scope,mode));
            _globalKeyPrefixDictionary.Clear();
            _keyPrefixToFullKeyListDictionary.Clear();
            foreach(var item in _allMappingsSet)
            {
                var (mappingKeys, mappingActions, mappingScope, mappingMode) = item;
                RecalculateAction(mappingKeys, mappingActions, mappingScope, mappingMode);
            }
        }
    }
}