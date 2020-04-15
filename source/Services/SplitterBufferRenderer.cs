using System.Linq;
using Terminal.Gui;
using myotui.Models.Config;
using myotui.Services;
using Autofac.Features.Indexed;
using System;
using myotui.Models;
using myotui.Views;

namespace myotui.Services
{
    public class SplitterBufferRenderer : IBufferRenderer
    {
        public View Layout(ViewNode node)
        {
            return node.View;
        }

        public void RegisterBindings(ViewNode node)
        {
            return;
        }
        public void RemoveBindings(ViewNode node)
        {
            return;
        }

        public void RegisterEvents(ViewNode node)
        {
            return;
        }

        public View Render(ViewNode node)
        {
            var view = new Splitter();
            node.View = view;
            return view;
        }
    }
}