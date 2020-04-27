using System.Collections.Generic;
namespace myotui.Models.Config
{
    public class VActionListBuffer : Buffer
    {
        public override bool Closable {get; set;} = false;
        public override bool Focusable {get; set;} = true;
        public bool HasSearch {get; set;} = true;
    }
}