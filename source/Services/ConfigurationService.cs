using System.IO;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Autofac;
using myotui.Models.Config;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using HandlebarsDotNet;
using System.Runtime.InteropServices;

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

            var runtime = Handlebars.Create();

            runtime.RegisterHelper("exists", ( writer, options, context, arguments ) =>
            {
                if(context.ContainsKey(arguments[0]) && context[arguments[0]] != null)
                {
                    options.Template(writer, context);
                }
            });
            runtime.RegisterHelper("json", ( writer, context, arguments ) =>
            {
                var jsonString = JsonConvert.SerializeObject(context[arguments[0]]) as string;
                writer.WriteSafeString(jsonString);
            });
            runtime.RegisterHelper("yaml", ( writer, context, arguments ) =>
            {
                var serializer = new YamlDotNet.Serialization.Serializer();
                serializer.Serialize(writer,context[arguments[0]]);
            });

            var partials = GetPartialsFromDirectory(configDirectory);
            partials.Keys.ToList().ForEach(key => runtime.RegisterTemplate(key,partials[key]));

            dynamic vars = new {};
            var varsPath = Path.Join(dirFullPath,@"vars.yml");
            if(File.Exists(varsPath))
            {
                var varsContent = await File.ReadAllTextAsync(varsPath);
                var varsTemplate = runtime.Compile(varsContent);

                var varsDeserializer = new Deserializer();
                vars = varsDeserializer.Deserialize<dynamic>(varsTemplate(new {}));
            }


            var mainPath = Path.Join(dirFullPath,@"main.yml");
            var mainContent = await File.ReadAllTextAsync(mainPath);

            var mainTemplate = runtime.Compile(mainContent);
            var renderedMain = mainTemplate(vars);

            var deserializer = new DeserializerBuilder()
                .WithNodeTypeResolver(_resolver)
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            _app =  deserializer.Deserialize<App>(renderedMain);
        }

        private IDictionary<string,string> GetPartialsFromDirectory(string directoryPath)
        {
            return Directory
                .EnumerateFiles(directoryPath,"*", SearchOption.AllDirectories)
                .ToDictionary(
                    filePath => FixWindowsPathSeparator(Path.GetRelativePath(directoryPath,filePath)),
                    filePath => File.ReadAllText(filePath)
                );
        }

        public IBuffer GetBufferByName(string name)
        {
            return _app.Buffers.FirstOrDefault(x => x.Name == name);
        }

        private string FixWindowsPathSeparator(string input)
        {
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            return isWindows ? input.Replace(@"\","/") : input;
        }
        
    }
}