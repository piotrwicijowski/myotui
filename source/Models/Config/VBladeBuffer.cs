using System.Collections.Generic;
namespace myotui.Models.Config
{
    public class VBladeBuffer : LayoutBuffer
    {
        public override bool AutoSplitters {get; set;} = true;
        public override bool Closable {get; set;} = false;
        public override bool Focusable {get; set;} = true;
    }
}