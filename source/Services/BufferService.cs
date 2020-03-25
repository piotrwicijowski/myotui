using System.Linq;
using Autofac;
using Terminal.Gui;
using myotui.Models.Config;
using Autofac.Features.Indexed;
using System;
using myotui.Models;

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

        public View RenderNode(ViewNode node)
        {
            var parentRenderer = _bufferRenderers[node.Buffer.GetType()];
            parentRenderer.Render(node);
            node.Children?.ToList().ForEach(childNode => RenderNode(childNode));
            parentRenderer.RegisterEvents(node);
            parentRenderer.RegisterBindings(node);
            return parentRenderer.Layout(node);
        }

        public IBuffer GetBufferByName(string name)
        {
            return _configuration.AppConfiguration.Buffers.FirstOrDefault(x => x.Name == name);
        }
         
    }
}