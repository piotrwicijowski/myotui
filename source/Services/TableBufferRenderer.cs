using System.Linq;
using Terminal.Gui;
using myotui.Models;
using myotui.Services;
using Autofac.Features.Indexed;
using System;

namespace myotui.Services
{
    public class TableBufferRenderer : IBufferRenderer
    {
        protected readonly IBufferService _bufferService;
        protected readonly IIndex<Type,IRawContentService> _rawContentServices;
        protected readonly IIndex<ValueMapType,IContentMapService> _maps;
        public TableBufferRenderer(IBufferService bufferService, IIndex<Type,IRawContentService> rawContentServices, IIndex<ValueMapType,IContentMapService> maps)
        {
            _bufferService = bufferService;
            _rawContentServices = rawContentServices;
            _maps = maps;
        }
        public View Render(IBuffer buffer, string scope)
        {
            var tablebuffer = buffer as TableBuffer;
            var view = new FrameView("");
            var rawContentService = _rawContentServices[tablebuffer.Content.GetType()];
            var rawContent = rawContentService.GetRawOutput(tablebuffer.Content);
            var map = _maps[tablebuffer.Content.Map];
            var content = map.MapRawData(rawContent);
            var listView = new ListView(content.ToList());
            listView.X = 0;
            listView.Y = 0;
            listView.Width = Dim.Fill();
            listView.Height = Dim.Fill();
            view.Add(listView);
            return view;
        }
    }
}