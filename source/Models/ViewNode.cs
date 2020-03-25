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

        public IList<ViewNode> Children {get; set;}
        public View View {get; set;}
        
        public void FocusNextChild()
        {
            // if(!View.HasFocus) {return;}
            if(!Children.Any()) {return;} 
            var focusedChildIndex = Children.Select(child => child.View).ToList().IndexOf(this.View.Focused);
            if(focusedChildIndex < Children.Count - 1) {View.SetFocus(Children.ToList()[focusedChildIndex + 1].View);}
            if(focusedChildIndex == Children.Count - 1) {Parent?.FocusNextChild();}
        }

        public void FocusPreviousChild()
        {
            // if(!View.HasFocus) {return;}
            if(!Children.Any()) {return;} 
            var focusedChildIndex = Children.Select(child => child.View).ToList().IndexOf(this.View.Focused);
            if(focusedChildIndex > 0) {View.SetFocus(Children.ToList()[focusedChildIndex - 1].View);}
            if(focusedChildIndex == 0) {Parent?.FocusPreviousChild();}
        }
        // public SizeHint Width {get; set;}

    }
}