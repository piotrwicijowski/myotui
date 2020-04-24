using System.Collections.Generic;
using System.ComponentModel;

namespace myotui.Models.Config
{
    public class ListValueContent : ValueContent
    {
        public IEnumerable<string> Value {get; set;}
        [DefaultValue(ValueMapType.string_to_string_array)]
        public override IList<ValueMapType> Maps {get; set;} = new List<ValueMapType>(){ValueMapType.string_to_string_array};
        public IEnumerable<string> Select {get; set;}
        public IEnumerable<string> Order {get; set;}
    }
}