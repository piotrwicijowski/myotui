using System;
using Terminal.Gui;
using Unix.Terminal;

namespace myotui.Views
{
    public class Splitter : View
    {
        public override void Redraw(Rect rect)
        {
            var plusRune = Driver.GetType() switch
            {
                {Name : "CursesDriver"} => new Rune(Curses.ACS_PLUS),
                {Name : "WindowsDriver"} => new Rune('\u253c'),
                {Name : "NetDriver"} => new Rune('\u253c'),
                _ => new Rune('+')
            };
			Move (0, 0);
            switch(rect)
            {
                case Rect r when r.Width == 1 && r.Height == 1:
                    Driver.AddRune(plusRune);
                    break;
                case Rect r when r.Width == 1:
                    for(int i = 0; i < r.Height; ++i)
                    {
                        Move (0, i);
                        Driver.AddRune(Driver.VLine);
                    }
                    break;
                case Rect r when r.Height == 1:
                    for(int i = 0; i < r.Width; ++i)
                    {
                        Driver.AddRune(Driver.HLine);
                    }
                    break;
                case Rect r:
                    Move (0, 0);
                    Driver.AddRune(Driver.ULCorner);
                    Move (r.Width - 1, 0);
                    Driver.AddRune(Driver.URCorner);
                    Move (0, r.Height - 1);
                    Driver.AddRune(Driver.LLCorner);
                    Move (r.Width - 1, r.Height - 1);
                    Driver.AddRune(Driver.LRCorner);
                    for(int i = 1; i < r.Height - 1; ++i)
                    {
                        Move (0, i);
                        Driver.AddRune(Driver.VLine);
                        Move (r.Width - 1, i);
                        Driver.AddRune(Driver.VLine);
                    }
                    for(int i = 1; i < r.Width - 1; ++i)
                    {
                        Move (i, 0);
                        Driver.AddRune(Driver.HLine);
                        Move (i, r.Height - 1);
                        Driver.AddRune(Driver.HLine);
                    }
                    break;
            }
            base.Redraw(rect);
        }
    }
}