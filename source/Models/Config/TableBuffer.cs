using System.Collections.Generic;
namespace myotui.Models.Config
{
    public class TableBuffer : IBuffer
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IEnumerable<IBinding> Bindings {get; set;}
        public IEnumerable<IParameter> Parameters {get; set;}
        public IValueContent Content {get; set;}
        public IList<ColumnDefinition> Columns {get; set;} = new List<ColumnDefinition>();
    }
}