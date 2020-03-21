using System.Collections.Generic;
using ConsoleFramework;
using ConsoleFramework.Controls;

namespace myotui.Models
{
    public interface ILayout
    {
        public Panel BuildLayout()
        {
            return new Panel();
        }
    }
}