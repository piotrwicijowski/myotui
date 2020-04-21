using System.Collections.Generic;
using System.ComponentModel;

namespace myotui.Models.Config
{
    public interface IValueContent
    {
        public IList<ValueMapType> Maps {get; set;}
    }
}