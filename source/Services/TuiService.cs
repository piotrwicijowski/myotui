using System.Linq;
using Terminal.Gui;
using myotui.Models.Config;
using myotui.Views;
using myotui.Models;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Autofac.Features.Indexed;

namespace myotui.Services
{
    public class TuiService : ITuiService
    {
        protected readonly IConfigurationService _configuration;
        protected readonly IBufferService _bufferSerivce;
        protected readonly IKeyService _keyService;
        protected readonly INodeService _nodeService;
        protected readonly IActionService _actionService;
        protected readonly IModeService _modeService;
        protected readonly IIndex<IOServiceType,IIOService> _ioSerivces;

        public TuiService(IConfigurationService configuration, IBufferService bufferService, IKeyService keyService, INodeService nodeService, IActionService actionService, IModeService modeService, IIndex<IOServiceType, IIOService> ioSerivces)
        {
            _configuration = configuration;
            _bufferSerivce = bufferService;
            _keyService = keyService;
            _nodeService = nodeService;
            _actionService = actionService;
            _modeService = modeService;
            _ioSerivces = ioSerivces;
        }

        public void Run()
        {
            var rootBuffer = _configuration.GetBufferByName("root");
            _modeService.DefaultMode = _configuration.AppConfiguration?.Modes?.FirstOrDefault()?.Name ?? "normal";
            _modeService.CurrentMode = _modeService.DefaultMode;
            _nodeService.RootNode = _nodeService.BuildNodeTree(rootBuffer,"/root");
            Terminal.Gui.Application.Init();
            var window = BuildWindow(_nodeService.RootNode);
            var top = Terminal.Gui.Application.Top;
            RegisterApplicationActions();
            top.Add(window);
            var curserDriver = Terminal.Gui.Application.Driver as CursesDriver;
            if(curserDriver!= null)
            {
                Unix.Terminal.Curses.nonl();
            }
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
                    HotFocus = Attribute.Make(Color.Black, Color.White),
                    HotNormal = Attribute.Make(Color.White, Color.Black),
                    // HotFocus = Attribute.Make(Color.BrightBlue, Color.Brown),
                    // HotNormal = Attribute.Make(Color.Red, Color.BrightRed),
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
         
        public void RegisterApplicationActions()
        {
            _actionService.RegisterAction($"quit","/**",(_) => { Terminal.Gui.Application.RequestStop();return true;});
            _actionService.RegisterAction($"clearKeyStack","/**",(_) => { _keyService.ClearStack();return true;});
            _actionService.RegisterAction($"yank","/**",(text) => {
                Task.Run(async () => await TextCopy.Clipboard.SetText(text)).Wait();
                return true;
            });
            _actionService.RegisterAction($"setMode","/**",(mode) => { _modeService.CurrentMode = mode;return true;});
            _actionService.RegisterAction($"nop","/**",(_) => { return true;});
            _actionService.RegisterAction($"openUrlInBrowser","/**",(url) => { return OpenUrl(url);});
            _actionService.RegisterAction($"cli","/**",(commandline) => { _ioSerivces[IOServiceType.Cli].Run(commandline); return true;});
        }

        private bool OpenUrl(string url)
        {
            try
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}