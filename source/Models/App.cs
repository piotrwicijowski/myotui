using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;
using Autofac;
using myotui.Services;

namespace myotui.Models
{
    public class App : IApp
    {
        protected readonly IConfigurationService _configuration;

        public App() {}
        public App(IConfigurationService configuration)
        {
            _configuration = configuration;
        }

        public string Name {get; set;}

        public IEnumerable<IModeDefinition> Modes {get; set;}
        public IEnumerable<IBuffer> Buffers {get; set;}


    }
}