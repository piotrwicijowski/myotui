﻿using System;
using System.IO;
using System.Threading.Tasks;
using myotui.Models;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using ConsoleFramework;
using ConsoleFramework.Controls;
using ConsoleFramework.Core;
using ConsoleFramework.Events;

namespace myotui
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var collection = new ServiceCollection();
            var builder = new ContainerBuilder();
            builder.RegisterType<App>().As<IApp>();
            builder.RegisterType<BufferContent>().As<IContent>();
            builder.RegisterType<CliContent>().As<IContent>();
            builder.RegisterType<TableBuffer>().As<IBuffer>();
            builder.RegisterType<HBoxBuffer>().As<IBuffer>();
            builder.RegisterType<VBoxBuffer>().As<IBuffer>();
            builder.RegisterType<HBladeBuffer>().As<IBuffer>();
            builder.RegisterType<VBladeBuffer>().As<IBuffer>();
            builder.RegisterType<ModeDefinition>().As<IModeDefinition>();
            builder.Populate(collection);
            var appContainer = builder.Build();
            var serviceContainer = new AutofacServiceProvider(appContainer);

            var fileStream = new FileStream("config/app2.yml", FileMode.Open);
            using var reader = new StreamReader(fileStream);
            var fileContent = await reader.ReadToEndAsync();
            var deserializer = new DeserializerBuilder()
                .WithNodeTypeResolver(new AutofacResolver(serviceContainer))
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var app = deserializer.Deserialize<App>(fileContent);


            var windowsHost = new WindowsHost(){
                Name = app.Name
            };
            var window = app.BuildWindow();
            windowsHost.Show(window);
            // WindowsHost windowsHost = ( WindowsHost ) ConsoleApplication.LoadFromXaml( "host.xml", null );
            // ConsoleFramework.Controls.Window mainWindow = (ConsoleFramework.Controls.Window) ConsoleApplication.LoadFromXaml( 
            //     "app.xml", null );
            // windowsHost.Show( mainWindow );

            ConsoleApplication.Instance.Run( windowsHost );
            ConsoleApplication.Instance.Maximize();
            
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}