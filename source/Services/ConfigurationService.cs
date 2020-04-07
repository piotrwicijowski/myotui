using System.IO;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Autofac;
using myotui.Models.Config;
using System.Linq;
using DotLiquid;
using DotLiquid.FileSystems;

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

        public async Task BuildAppConfiguration(string configDirectory)
        {
            var dirFullPath = Path.GetFullPath(configDirectory);
            var mainPath = Path.Join(dirFullPath,@"main.yml");
            var fileStream = new FileStream(mainPath, FileMode.Open);
            using var reader = new StreamReader(fileStream);
            var fileContent = await reader.ReadToEndAsync();
            Template.FileSystem = new LocalFileSystem(dirFullPath);
            var configTemplate = Template.Parse(fileContent);
            var renderedConfig = configTemplate.Render();
            var deserializer = new DeserializerBuilder()
                .WithNodeTypeResolver(_resolver)
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            _app =  deserializer.Deserialize<App>(renderedConfig);
        }
        public IBuffer GetBufferByName(string name)
        {
            return _app.Buffers.FirstOrDefault(x => x.Name == name);
        }
        
    }
}