using System.Collections.Generic;
using System.ComponentModel;

namespace myotui.Models.Config
{
    public class ActionsValueContent : ValueContent
    {
        public IEnumerable<ActionListElement> Value {get; set;}
        [DefaultValue(ValueMapType.json_array_to_table)]

        public override IList<ValueMapType> Maps {get; set;} = new List<ValueMapType>(){ValueMapType.json_array_to_table};
    }

}