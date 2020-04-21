using System.Collections.Generic;
using System.ComponentModel;

namespace myotui.Models.Config
{
    public class ActionListValueContent : IValueContent
    {
        public IEnumerable<ActionListElement> Value {get; set;}
        [DefaultValue(ValueMapType.json_array_to_table)]

        public IList<ValueMapType> Maps {get; set;} = new List<ValueMapType>(){ValueMapType.json_array_to_table};
    }

}