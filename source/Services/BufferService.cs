using System.Linq;
using Autofac;
using Terminal.Gui;
using myotui.Models;

namespace myotui.Services
{
    public class BufferService : IBufferService
    {

        private readonly IComponentContext _context;
        protected readonly IConfigurationService _configuration;

        public BufferService(IComponentContext context, IConfigurationService configuration)
        {
            _configuration = configuration;
            _context = context;
        }
        public View RenderBuffer(string name)
        {
            var buffer = _configuration.AppConfiguration.Buffers.FirstOrDefault(x => x.Name == name);
            var bufferRenderer = _context.ResolveNamed<IBufferRenderer>(buffer.GetType().Name);
            return bufferRenderer.Render(buffer);
        }
         
    }
}