using System.Collections.Generic;
using Terminal.Gui;
using System.Linq;

namespace myotui.Models.Config
{
    public class StackBuffer : LayoutBuffer
    {
        public override bool Closable {get; set;} = false;
        public override bool Focusable {get; set;} = true;
    }
}