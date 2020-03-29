using System.IO;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Autofac;
using myotui.Models.Config;
using System.Linq;

namespace myotui.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly INodeTypeResolver _resolver;

        private App _app;
        public App AppConfiguration {get => _app; }
        public ConfigurationService(INodeTypeResolver resolver)
        {
            _resolver = resolver;
        }

        public async Task BuildAppConfiguration(string configPath)
        {
            var fileStream = new FileStream(configPath, FileMode.Open);
            using var reader = new StreamReader(fileStream);
            var fileContent = await reader.ReadToEndAsync();
            var deserializer = new DeserializerBuilder()
                .WithNodeTypeResolver(_resolver)
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            _app =  deserializer.Deserialize<App>(fileContent);
        }
        public IBuffer GetBufferByName(string name)
        {
            return _app.Buffers.FirstOrDefault(x => x.Name == name);
        }
        
    }
}