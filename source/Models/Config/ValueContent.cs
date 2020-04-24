using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace myotui.Models.Config
{
    public class ValueContent : IValueContent
    {
        public virtual ValueMapType Map {
            get => (ValueMapType)(Maps?.FirstOrDefault());
            set 
            {
                Maps = new List<ValueMapType>();
                Maps.Add(value);
            }
        }
        public virtual IList<ValueMapType> Maps {get; set;}
    }
}