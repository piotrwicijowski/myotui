using System.IO;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Autofac;
using myotui.Models.Config;
using System.Linq;
using System.Collections.Generic;
using HandlebarsDotNet;

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
            var mainContent = await reader.ReadToEndAsync();

            var partials = GetPartialsFromDirectory(configDirectory);
            partials.Keys.ToList().ForEach(key => Handlebars.RegisterTemplate(key,partials[key]));

            var mainTemplate = Handlebars.Compile(mainContent);
            var renderedMain = mainTemplate(new {});

            var deserializer = new DeserializerBuilder()
                .WithNodeTypeResolver(_resolver)
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            _app =  deserializer.Deserialize<App>(renderedMain);
        }

        private IDictionary<string,string> GetPartialsFromDirectory(string directoryPath)
        {
            return Directory.EnumerateFiles(directoryPath,"*", SearchOption.AllDirectories).ToDictionary(filePath => Path.GetRelativePath(directoryPath,filePath), filePath =>
                File.ReadAllText(filePath));
        }

        public IBuffer GetBufferByName(string name)
        {
            return _app.Buffers.FirstOrDefault(x => x.Name == name);
        }
        
    }
}