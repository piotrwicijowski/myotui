using System;
using System.IO;
using System.Threading.Tasks;
using myotui.Models;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Autofac;

namespace myotui
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var collection = new ServiceCollection();
            var builder = new ContainerBuilder();
            builder.RegisterType<App>().As<IApp>();
            builder.RegisterType<Models.Buffer>().As<IBuffer>();
            builder.RegisterType<Content>().As<IContent>();
            builder.RegisterType<Layout>().As<ILayout>();
            builder.RegisterType<ModeDefinition>().As<IModeDefinition>();
            builder.RegisterType<Window>().As<IWindow>();
            builder.Populate(collection);
            var appContainer = builder.Build();
            var serviceContainer = new AutofacServiceProvider(appContainer);

            var fileStream = new FileStream("config/app1.yml", FileMode.Open);
            using var reader = new StreamReader(fileStream);
            var fileContent = await reader.ReadToEndAsync();
            var deserializer = new DeserializerBuilder()
                .WithNodeTypeResolver(new AutofacResolver(serviceContainer))
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var app = deserializer.Deserialize<App>(fileContent);
            Console.WriteLine("Hello World!");
        }
    }
}
