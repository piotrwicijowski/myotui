using System.Threading.Tasks;
using myotui.Services;
using myotui.Models.Config;
namespace myotui
{
    public class Application
    {
        protected readonly IConfigurationService _configuration;
        protected readonly ITuiService _tuiService;

        public Application(IConfigurationService configuration, ITuiService tuiService)
        {
            _configuration = configuration;
            _tuiService = tuiService;
        }

        public async Task Run()
        {
            await _configuration.BuildAppConfiguration("config/app.yml");

            _tuiService.Run();
        }
    }
}