using System.Collections.Generic;
namespace myotui.Models.Config
{
    public class SplitterBuffer : Buffer
    {
        public override bool Closable {get; set;} = false;
        public override bool Focusable {get; set;} = false;
    }
}