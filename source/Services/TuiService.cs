using System.Linq;
using Terminal.Gui;
using myotui.Models.Config;
using myotui.Views;
using myotui.Models;

namespace myotui.Services
{
    public class TuiService : ITuiService
    {
        protected readonly IConfigurationService _configuration;
        protected readonly IBufferService _bufferSerivce;
        protected readonly IKeyService _keyService;
        protected readonly INodeService _nodeService;
        private ViewNode _rootNode;

        public TuiService(IConfigurationService configuration, IBufferService bufferService, IKeyService keyService, INodeService nodeService)
        {
            _configuration = configuration;
            _bufferSerivce = bufferService;
            _keyService = keyService;
            _nodeService = nodeService;
        }

        public void Run()
        {
            var rootBuffer = _bufferSerivce.GetBufferByName("root");
            _rootNode = _nodeService.BuildNodeTree(rootBuffer,"/root");
            Terminal.Gui.Application.Init();
            var window = BuildWindow(_rootNode);
            var top = Terminal.Gui.Application.Top;
            top.Add(window);
            Terminal.Gui.Application.Run();
        }


        public View BuildWindow(ViewNode node)
        {
            var window = new KeyedView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = new ColorScheme()
                {
                    Focus = Attribute.Make(Color.Black, Color.White),
                    Normal = Attribute.Make(Color.White, Color.Black),
                    HotFocus = Attribute.Make(Color.BrightBlue, Color.Brown),
                    HotNormal = Attribute.Make(Color.Red, Color.BrightRed),
                },

            };
            window.KeyPressed += (key) => _keyService.ProcessKeyEvent(key, node);
            var label = new Label(_configuration.AppConfiguration.Name)
            {
                X = 0 + 1,
                Y = 0,
            };
            var rootView = _bufferSerivce.RenderNode(node);
            rootView.X = 0;
            rootView.Y = 1;
            rootView.Width = Dim.Fill();
            rootView.Height = Dim.Fill();

            window.Add(label, rootView);
            return window;
        }
         
    }
}