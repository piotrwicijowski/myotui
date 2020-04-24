using System.Collections.Generic;
using System.ComponentModel;

namespace myotui.Models.Config
{
    public interface IValueContent
    {
        public ValueMapType Map {get; set;}
        public IList<ValueMapType> Maps {get; set;}
    }
}