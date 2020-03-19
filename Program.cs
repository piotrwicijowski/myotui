using System;
using System.IO;
using System.Threading.Tasks;
using myotui.Models;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace myotui
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var fileStream = new FileStream("config/app1.yml", FileMode.Open);
            using var reader = new StreamReader(fileStream);
            var fileContent = await reader.ReadToEndAsync();
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var app = deserializer.Deserialize<App>(fileContent);
            Console.WriteLine("Hello World!");
        }
    }
}
