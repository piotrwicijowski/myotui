﻿using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using myotui.Models.Config;
using myotui.Services;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Terminal.Gui;
using myotui.Models;
using Autofac.Features.AttributeFilters;

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
            builder.RegisterType<ActionService>().As<IActionService>().SingleInstance();
            builder.RegisterType<KeyService>().As<IKeyService>().SingleInstance();
            builder.RegisterType<NodeService>().As<INodeService>().SingleInstance();
            builder.RegisterType<BufferService>().As<IBufferService>();
            builder.RegisterType<ScopeService>().As<IScopeService>();
            builder.RegisterType<ModeService>().As<IModeService>().SingleInstance();
            builder.RegisterType<ParameterService>().As<IParameterService>();
            builder.RegisterType<RefRawContentService>().Keyed<IRawContentService>(typeof(RefValueContent));
            builder.RegisterType<CliRawContentService>().Keyed<IRawContentService>(typeof(CliValueContent)).WithAttributeFiltering();
            builder.RegisterType<ListRawContentService>().Keyed<IRawContentService>(typeof(ListValueContent));
            builder.RegisterType<ActionListRawContentService>().Keyed<IRawContentService>(typeof(ActionsValueContent));
            builder.RegisterType<NopMapService>().Keyed<IContentMapService>(ValueMapType.nop);
            builder.RegisterType<JsonArrayMapService>().Keyed<IContentMapService>(ValueMapType.json_array_to_table);
            builder.RegisterType<JsonObjectMapService>().Keyed<IContentMapService>(ValueMapType.json_object_to_dict);
            builder.RegisterType<StringArrayMapService>().Keyed<IContentMapService>(ValueMapType.string_to_string_array);

            builder.RegisterType<CliIOService>().Keyed<IIOService>(IOServiceType.Cli);

            builder.RegisterType<VBoxBufferRenderer>().Keyed<IBufferRenderer>(typeof(VBoxBuffer));
            builder.RegisterType<HBoxBufferRenderer>().Keyed<IBufferRenderer>(typeof(HBoxBuffer));
            builder.RegisterType<HBladeBufferRenderer>().Keyed<IBufferRenderer>(typeof(HBladeBuffer));
            builder.RegisterType<StackBufferRenderer>().Keyed<IBufferRenderer>(typeof(StackBuffer));
            builder.RegisterType<VActionListBufferRenderer>().Keyed<IBufferRenderer>(typeof(VActionListBuffer));
            builder.RegisterType<HActionListBufferRenderer>().Keyed<IBufferRenderer>(typeof(HActionListBuffer));
            builder.RegisterType<TableBufferRenderer>().Keyed<IBufferRenderer>(typeof(TableBuffer));
            builder.RegisterType<DictBufferRenderer>().Keyed<IBufferRenderer>(typeof(DictBuffer));
            builder.RegisterType<SplitterBufferRenderer>().Keyed<IBufferRenderer>(typeof(SplitterBuffer));

            builder.RegisterType<App>().As<IApp>();
            builder.RegisterType<BufferLayoutContent>().As<ILayoutContent>();
            builder.RegisterType<CliValueContent>().As<IValueContent>();
            builder.RegisterType<RefValueContent>().As<IValueContent>();
            builder.RegisterType<ActionsValueContent>().As<IValueContent>();
            builder.RegisterType<ListValueContent>().As<IValueContent>();
            builder.RegisterType<TableBuffer>().As<IBuffer>();
            builder.RegisterType<VActionListBuffer>().As<IBuffer>();
            builder.RegisterType<HActionListBuffer>().As<IBuffer>();
            builder.RegisterType<DictBuffer>().As<IBuffer>();
            builder.RegisterType<SplitterBuffer>().As<IBuffer>();
            builder.RegisterType<HBoxBuffer>().As<IBuffer>();
            builder.RegisterType<VBoxBuffer>().As<IBuffer>();
            builder.RegisterType<HBladeBuffer>().As<IBuffer>();
            builder.RegisterType<VBladeBuffer>().As<IBuffer>();
            builder.RegisterType<StackBuffer>().As<IBuffer>();
            builder.RegisterType<ModeDefinition>().As<IModeDefinition>();
            builder.RegisterType<Parameter>().As<IParameter>();
            builder.RegisterType<Binding>().As<IBinding>();

            builder.RegisterType<KeyCodeMap>().As<IKeyCodeMap>().SingleInstance();
            builder.RegisterType<KeyPrefixDictionary>();
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
