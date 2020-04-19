using System.Linq;
using myotui.Models;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Services
{
    public class StackBufferRenderer : LayoutBufferRenderer
    {
        public StackBufferRenderer(IActionService actionService, IKeyService keyService, IBufferService bufferService) : base(actionService, keyService, bufferService)
        {
        }

        public override View Layout(ViewNode node)
        {
            var view = node.View;
            view.RemoveAll();
            view.Clear();
            var currentChildViewNode = node.Children?.LastOrDefault()?.View;
            if(currentChildViewNode != null)
            {
                currentChildViewNode.X = 0;
                currentChildViewNode.Y = 0;
                currentChildViewNode.Width = Dim.Fill();
                currentChildViewNode.Height = Dim.Fill();
                view.Add(currentChildViewNode);
            }
            return view;
        }

        protected override void RegisterFocusAction(ViewNode node)
        {
            node.RegisteredActions.Add(_actionService.RegisterAction($"{node.Scope}.focus","/**",(_) => {node.Parent?.View.SetFocus(node.View);return true;}));
        }
    }
}