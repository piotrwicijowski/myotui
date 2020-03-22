using System.Threading.Tasks;
using myotui.Services;
using myotui.Models;
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
            await _configuration.BuildAppConfiguration("config/app3.yml");

            _tuiService.Run();
        }
    }
}