using System.Collections.Generic;

namespace myotui.Models
{
    public interface IApp
    {
        public string Name {get; set;}

        public IEnumerable<IModeDefinition> Modes {get; set;}
        public IEnumerable<IBuffer> Buffers {get; set;}
    }
}