namespace myotui.Services
{
    public interface IScopeService
    {
         
        public bool IsInScope(string currentScope, string actionScope);

        public int ScopeDepth(string scope);

        public string ResolveRelativeScope(string baseScope, string relativeScope);

        public string ResolveRelativeAction(string baseScope, string relativeAction);
    }
}