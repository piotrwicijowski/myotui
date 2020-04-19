using System;
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
        // public bool Focusable {get; set;} = true;
        public IList<ViewNode> Children {get; set;}
        public IDictionary<string, string> Parameters {get; set;}
        public View View {get; set;}
        public SizeHint Width {get; set;} = new SizeHint();
        public SizeHint Height {get; set;} = new SizeHint();
        public List<Guid> RegisteredActions = new List<Guid>();
        public ViewNode LastFocusedNode;
        public bool SkipKeyHandling {get; set;}
    }
}