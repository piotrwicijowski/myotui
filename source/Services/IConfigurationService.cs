using System.Threading.Tasks;
using myotui.Models;

namespace myotui.Services
{
    public interface IConfigurationService
    {
        public Task BuildAppConfiguration(string configPath);
        public App AppConfiguration {get; }
         
    }
}