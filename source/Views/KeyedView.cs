using System;
using Terminal.Gui;

namespace myotui.Views
{
    public class KeyedView : View
    {
        public Action<KeyEvent> KeyPressed;

        public override bool ProcessKey (KeyEvent keyEvent)
        {
            KeyPressed.Invoke(keyEvent);
            return true;
            // return base.ProcessKey (keyEvent);
        }
    }
}