using System.Collections.Generic;
using ConsoleFramework;
using ConsoleFramework.Controls;

namespace myotui.Models
{
    public interface IApp
    {
        public string Name {get; set;}

        public IEnumerable<IModeDefinition> Modes {get; set;}
        public IEnumerable<IBuffer> Buffers {get; set;}
        public Window BuildWindow();
    }
}