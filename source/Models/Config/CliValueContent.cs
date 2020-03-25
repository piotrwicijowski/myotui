﻿using System.Collections.Generic;
using System.ComponentModel;

namespace myotui.Models.Config
{
    public class CliValueContent : IValueContent
    {
        public string Input {get; set;}
        [DefaultValue(ValueMapType.string_to_string_array)]
        public ValueMapType Map {get; set;} = ValueMapType.string_to_string_array;
        public IEnumerable<string> Select {get; set;}
        public IEnumerable<string> Order {get; set;}
    }
}