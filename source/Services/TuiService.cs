using myotui.Models;

namespace myotui.Services
{
    public class TuiService : ITuiService
    {
        protected readonly IConfigurationService _configuration;
        protected readonly IApp _app;

        public TuiService(IConfigurationService configuration, IApp app)
        {
            _configuration = configuration;
            _app = app;
        }

        public void Run()
        {
            var window = _app.BuildWindow();
            Terminal.Gui.Application.Init();
            var top = Terminal.Gui.Application.Top;
            top.Add(window);
            Terminal.Gui.Application.Run();
        }
         
    }
}