using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using myotui.Models;
using myotui.Services;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Terminal.Gui;

namespace myotui
{
    class Program
    {
        private static IContainer CompositionRoot()
        {
            var collection = new ServiceCollection();
            var builder = new ContainerBuilder();
            builder.RegisterType<Application>();
            builder.RegisterType<AutofacResolver>().As<INodeTypeResolver>();
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>().SingleInstance();
            builder.RegisterType<TuiService>().As<ITuiService>();
            builder.RegisterType<ActionService>().As<IActionService>();
            builder.RegisterType<BufferService>().As<IBufferService>();
            builder.RegisterType<CliRawContentService>().Keyed<IRawContentService>(typeof(CliValueContent));
            builder.RegisterType<ListRawContentService>().Keyed<IRawContentService>(typeof(ListValueContent));
            builder.RegisterType<JsonArrayMapService>().Keyed<IContentMapService>(ValueMapType.json_array_to_table);
            builder.RegisterType<StringArrayMapService>().Keyed<IContentMapService>(ValueMapType.string_to_string_array);

            builder.RegisterType<VBoxBufferRenderer>().Keyed<IBufferRenderer>(typeof(VBoxBuffer));
            builder.RegisterType<HBoxBufferRenderer>().Keyed<IBufferRenderer>(typeof(HBoxBuffer));
            builder.RegisterType<HBladeBufferRenderer>().Keyed<IBufferRenderer>(typeof(HBladeBuffer));
            builder.RegisterType<TableBufferRenderer>().Keyed<IBufferRenderer>(typeof(TableBuffer));

            builder.RegisterType<App>().As<IApp>();
            builder.RegisterType<BufferLayoutContent>().As<ILayoutContent>();
            builder.RegisterType<CliValueContent>().As<IValueContent>();
            builder.RegisterType<ListValueContent>().As<IValueContent>();
            builder.RegisterType<TableBuffer>().As<IBuffer>();
            builder.RegisterType<HBoxBuffer>().As<IBuffer>();
            builder.RegisterType<VBoxBuffer>().As<IBuffer>();
            builder.RegisterType<HBladeBuffer>().As<IBuffer>();
            builder.RegisterType<VBladeBuffer>().As<IBuffer>();
            builder.RegisterType<ModeDefinition>().As<IModeDefinition>();
            builder.RegisterType<Binding>().As<IBinding>();
            builder.Populate(collection);
            var container = builder.Build();
            return container;
        }
        static async Task Main(string[] args)
        {
            await CompositionRoot().Resolve<Application>().Run();
        }
    }
}
