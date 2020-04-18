using Terminal.Gui;

namespace myotui
{
    public static class Globals
    {
        public static Color DefaultOrBlack {get {
                if(Terminal.Gui.Application.Driver is CursesDriver)
                {
                    return Color.Default;
                }
                else
                {
                    return Color.Black;
                }
            }
        }
    }
}