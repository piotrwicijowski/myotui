using System.Collections.Generic;
using System.Linq;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Models
{
    public class ViewNode
    {
        public string Scope {get; set;}
        public IBuffer Buffer {get;set;}
        public ViewNode Parent {get; set;}
        public bool Focusable {get; set;} = true;
        public IList<ViewNode> Children {get; set;}
        public IDictionary<string, string> Parameters {get; set;}
        public View View {get; set;}
        public SizeHint Width {get; set;} = new SizeHint();
        public SizeHint Height {get; set;} = new SizeHint();
        public readonly Dictionary<Key, (string action, string scope)> TriggerActionDictionary = new Dictionary<Key, (string action, string scope)>();

        public bool FocusNextChild()
        {
            if(!View.HasFocus) {return false;}
            if(Children == null || !Children.Any()) { return false; } 
            var focusedChildIndex = Children.Select(child => child.View).ToList().IndexOf(this.View.Focused);
            for(int i = focusedChildIndex + 1; i <= Children.Count - 1; ++i)
            {
                var child = Children.ToList()[i];
                if(child.Focusable)
                {
                    View.SetFocus(child.View);
                    return true;
                }
            }

            return false;
        }

        public bool FocusPreviousChild()
        {
            if(!View.HasFocus) {return false;}
            if(Children == null || !Children.Any()) { return false; } 
            var focusedChildIndex = Children.Select(child => child.View).ToList().IndexOf(this.View.Focused);
            for(int i = focusedChildIndex - 1; i >= 0; --i)
            {
                var child = Children.ToList()[i];
                if(child.Focusable)
                {
                    View.SetFocus(child.View);
                    return true;
                }
            }
            return false;
        }
    }
}