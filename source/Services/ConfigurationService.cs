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

            var partials = GetPartialsFromDirectory(configDirectory);
            partials.Keys.ToList().ForEach(key => Handlebars.RegisterTemplate(key,partials[key]));

            dynamic vars = new {};
            var varsPath = Path.Join(dirFullPath,@"vars.yml");
            if(File.Exists(varsPath))
            {
                var varsContent = await File.ReadAllTextAsync(varsPath);
                var varsTemplate = Handlebars.Compile(varsContent);

                var varsDeserializer = new Deserializer();
                vars = varsDeserializer.Deserialize<dynamic>(varsTemplate(new {}));
            }


            var mainPath = Path.Join(dirFullPath,@"main.yml");
            var mainContent = await File.ReadAllTextAsync(mainPath);

            var mainTemplate = Handlebars.Compile(mainContent);
            var renderedMain = mainTemplate(vars);

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