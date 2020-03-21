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

        private View GetRootContent()
        {
            var rootBuffer = _configuration.AppConfiguration.Buffers.FirstOrDefault(x => x.Name == "root");
            return rootBuffer.BuildLayout();
        }

        public View BuildWindow()
        {
            var rootContent = GetRootContent();
            var window = new View()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),

            };
            var label = new Label(_configuration.AppConfiguration.Name)
            {
                X = 0 + 1,
                Y = 0,
            };
            window.Add(label, rootContent);
            return window;
        }
    }
}