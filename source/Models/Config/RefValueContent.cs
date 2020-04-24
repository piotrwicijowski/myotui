using System.Collections.Generic;
using System.ComponentModel;

namespace myotui.Models.Config
{
    public class RefValueContent : ValueContent
    {
        public string Buffer {get; set;}
        [DefaultValue(ValueMapType.string_to_string_array)]
        public override IList<ValueMapType> Maps {get; set;} = new List<ValueMapType>(){ValueMapType.string_to_string_array};
    }
}