using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace myotui.Services
{
    public class ScopeService : IScopeService
    {
         
        public bool IsInScope(string currentScope, string actionScope)
        {
            var currentScopeDecoded = DecodeScopeString(currentScope);
            var actionScopeDecoded = DecodeScopeString(actionScope);
            var currentScopeEnumerator = currentScopeDecoded.GetEnumerator();
            var actionScopeEnumerator = actionScopeDecoded.GetEnumerator();
            currentScopeEnumerator.MoveNext();
            actionScopeEnumerator.MoveNext();
            var hasMore = true;
            var isWildcard = false;
            while(hasMore)
            {
                var currentValue = currentScopeEnumerator.Current;
                var actionValue = actionScopeEnumerator.Current;
                if(actionValue == "**")
                {
                    isWildcard = true;
                    var actionHasMore = actionScopeEnumerator.MoveNext();
                    if(!actionHasMore)
                    {
                        return true;
                    }
                    continue;
                }
                else if(isWildcard)
                {
                    if(currentValue == actionValue)
                    {
                        isWildcard = false;
                        continue;
                    }
                    var currentHasMore = currentScopeEnumerator.MoveNext();
                    if(!currentHasMore)
                    {
                        return false;
                    }
                    continue;
                }
                if(currentValue == actionValue || actionValue == "*")
                {
                    var currentHasMore = currentScopeEnumerator.MoveNext();
                    var actionHasMore = actionScopeEnumerator.MoveNext();
                    if(!currentHasMore && !actionHasMore)
                    {
                        return true;
                    }
                    continue;
                }
                hasMore = false;
                isWildcard = false;
            } 
            return false;
        }

        public int ScopeDepth(string scope)
        {
            //TODO create actual scope depth calculation
            return scope.Length;
        }
        public string ResolveRelativeAction(string baseScope, string relativeAction)
        {
            var isActionRelative = relativeAction.StartsWith(".");
            var resultAction = relativeAction;
            if(isActionRelative)
            {
                var relativeActionDecoded = DecodeScopeString(relativeAction);
                var relativeActionScope = relativeActionDecoded.SkipLast(1).ToList();
                var actionName = relativeActionDecoded.Last();
                string abosluteActionScope = ResolveRelativeScope(baseScope, $"{string.Join('/',relativeActionScope)}/");
                resultAction = $"{abosluteActionScope}.{actionName}";
            }
            return resultAction;
        }

        public string ResolveRelativeScope(string baseScope, string relativeScope)
        {
            var baseScopeDecoded = DecodeScopeString(baseScope);
            var relativeScopeDecoded = DecodeScopeString(relativeScope);
            var resultScopeDecoded = new List<string>();
            if(relativeScope.StartsWith("./") || relativeScope.StartsWith("../"))
            {
                resultScopeDecoded.AddRange(baseScopeDecoded);
            }
            foreach (var relativeScopeItem in relativeScopeDecoded)
            {
                if(relativeScopeItem == ".")
                {
                    continue;
                }
                if(relativeScopeItem == "..")
                {
                    resultScopeDecoded = resultScopeDecoded.SkipLast(1).ToList();
                    if(resultScopeDecoded.Count == 0)
                    {
                        break;
                    }
                    continue;
                }
                resultScopeDecoded.Add(relativeScopeItem);
            }
            return $"/{string.Join('/',resultScopeDecoded)}";
        }

        private static IList<string> DecodeScopeString(string scope)
        {
            return scope.Split('/').SkipWhile(val => val == "").Reverse().SkipWhile(val => val == "").Reverse().ToList();
        }
    }
}