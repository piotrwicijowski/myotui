using System.Linq;
using Terminal.Gui;
using myotui.Models;
using myotui.Services;
using Autofac.Features.Indexed;

namespace myotui.Services
{
    public class TableBufferRenderer : IBufferRenderer
    {
        protected readonly IBufferService _bufferService;
        protected readonly IRawContentService _rawContentService;
        protected readonly IIndex<CliMapType,IContentMapService> _maps;
        public TableBufferRenderer(IBufferService bufferService, IRawContentService rawContentService, IIndex<CliMapType,IContentMapService> maps)
        {
            _bufferService = bufferService;
            _rawContentService = rawContentService;
            _maps = maps;
        }
        public View Render(IBuffer buffer)
        {
            var tablebuffer = buffer as TableBuffer;
            var view = new FrameView(tablebuffer.Cli.Input);
            var rawContent = _rawContentService.GetRawOutput(tablebuffer.Cli.Input);
            var map = _maps[tablebuffer.Cli.Map];
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