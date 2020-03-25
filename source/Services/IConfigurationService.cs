using System.Threading.Tasks;
using myotui.Models.Config;

namespace myotui.Services
{
    public interface IConfigurationService
    {
        public Task BuildAppConfiguration(string configPath);
        public App AppConfiguration {get; }
         
    }
}