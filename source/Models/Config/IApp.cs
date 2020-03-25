using System.Collections.Generic;
using Terminal.Gui;

namespace myotui.Models.Config
{
    public interface IApp
    {
        public string Name {get; set;}

        public IEnumerable<IModeDefinition> Modes {get; set;}
        public IEnumerable<IBuffer> Buffers {get; set;}
    }
}