
using System.Collections.Generic;
using myotui.Models;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Services
{
    public interface IBufferRenderer
    {
        public View Render(ViewNode node);
        public View Layout(ViewNode node);
        public void RegisterEvents(ViewNode node);
        public void RegisterBindings(ViewNode node);
        public void RemoveBindings(ViewNode node);
    }
}