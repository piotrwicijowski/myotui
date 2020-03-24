using System.Linq;
using Terminal.Gui;
using myotui.Models;
using myotui.Views;

namespace myotui.Services
{
    public class TuiService : ITuiService
    {
        protected readonly IConfigurationService _configuration;
        protected readonly IBufferService _bufferSerivce;
        protected readonly IKeyService _keyService;

        public TuiService(IConfigurationService configuration, IBufferService bufferService, IKeyService keyService)
        {
            _configuration = configuration;
            _bufferSerivce = bufferService;
            _keyService = keyService;
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
            return _bufferSerivce.RenderBuffer("root","/root");
        }

        public View BuildWindow()
        {
            var window = new KeyedView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),

            };
            window.KeyPressed += _keyService.ProcessKeyEvent;
            var label = new Label(_configuration.AppConfiguration.Name)
            {
                X = 0 + 1,
                Y = 0,
            };
            var rootContent = GetRootContent();
            rootContent.X = 0;
            rootContent.Y = 1;
            rootContent.Width = Dim.Fill();
            rootContent.Height = Dim.Fill();

            window.Add(label, rootContent);
            return window;
        }
         
    }
}