using System.Linq;
using Autofac;
using Terminal.Gui;
using myotui.Models;
using Autofac.Features.Indexed;
using System;

namespace myotui.Services
{
    public class BufferService : IBufferService
    {

        protected readonly IConfigurationService _configuration;
        protected readonly IIndex<Type,IBufferRenderer> _bufferRenderers;
        public BufferService(IConfigurationService configuration, IIndex<Type,IBufferRenderer> bufferRenderers)
        {
            _configuration = configuration;
            _bufferRenderers = bufferRenderers;
        }

        public View RenderBuffer(string name)
        {
            var buffer = _configuration.AppConfiguration.Buffers.FirstOrDefault(x => x.Name == name);
            var bufferRenderer = _bufferRenderers[buffer.GetType()];
            return bufferRenderer.Render(buffer);
        }
         
    }
}