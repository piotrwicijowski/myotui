using System.Collections.Generic;
using System.ComponentModel;

namespace myotui.Models.Config
{
    public class RefValueContent : IValueContent
    {
        public string Buffer {get; set;}
        [DefaultValue(ValueMapType.string_to_string_array)]
        public IList<ValueMapType> Maps {get; set;} = new List<ValueMapType>(){ValueMapType.string_to_string_array};
    }
}