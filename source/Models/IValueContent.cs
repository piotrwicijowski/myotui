using System.Collections.Generic;
using System.ComponentModel;

namespace myotui.Models
{
    public interface IValueContent
    {
        public ValueMapType Map {get; set;}
        public IEnumerable<string> Select {get; set;}
        public IEnumerable<string> Order {get; set;}
    }
}