using System.Collections.Generic;
using System.ComponentModel;

namespace myotui.Models.Config
{
    public class CliValueContent : IValueContent
    {
        public string Input {get; set;}
        [DefaultValue(ValueMapType.string_to_string_array)]
        public IList<ValueMapType> Maps {get; set;} = new List<ValueMapType>(){ValueMapType.string_to_string_array};
    }
}