using System.Collections.Generic;
namespace myotui.Models.Config
{
    public class DictBuffer : Buffer
    {
        public IList<ColumnDefinition> Columns {get; set;} = new List<ColumnDefinition>();
    }
}