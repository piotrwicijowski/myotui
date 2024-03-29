﻿using System.Collections.Generic;
using Terminal.Gui;

namespace myotui.Models.Config
{
    public class BufferLayoutContent : ILayoutContent
    {
        public string Name {get; set;}
        public string Value {get; set;}
        public SizeHint Width {get; set;} = new SizeHint();
        public SizeHint Height {get; set;} = new SizeHint();
        public IEnumerable<IParameter> Parameters {get; set;}

    }
}