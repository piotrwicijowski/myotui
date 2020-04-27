using System.Collections.Generic;
namespace myotui.Models.Config
{
    public class TableBuffer : Buffer
    {
        public IList<ColumnDefinition> Columns {get; set;} = new List<ColumnDefinition>();
        public bool HasHeader {get; set;} = true;
        public bool HasSearch {get; set;} = false;
        public override bool Closable {get; set;} = true;
        public override bool Focusable {get; set;} = true;
    }
}