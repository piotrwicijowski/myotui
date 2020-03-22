using System.Linq;
using Terminal.Gui;
using myotui.Models;

namespace myotui.Services
{
    public class TuiService : ITuiService
    {
        protected readonly IConfigurationService _configuration;
        protected readonly IBufferService _bufferSerivce;

        public TuiService(IConfigurationService configuration, IBufferService bufferService)
        {
            _configuration = configuration;
            _bufferSerivce = bufferService;
        }

        public void Run()
        {
            var window = BuildWindow();
            Terminal.Gui.Application.Init();
            var top = Terminal.Gui.Application.Top;
            top.Add(window);
            Terminal.Gui.Application.Run();
        }
        private View GetRootContent()
        {
            return _bufferSerivce.RenderBuffer("root");
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