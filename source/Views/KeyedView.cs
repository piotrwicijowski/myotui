using System;
using Terminal.Gui;

namespace myotui.Views
{
    public class KeyedView : View
    {
        public Func<KeyEvent,bool> KeyPressed;

        public override bool ProcessKey (KeyEvent keyEvent)
        {
            var wasHandled = KeyPressed.Invoke(keyEvent);
            if(!wasHandled){
                return base.ProcessKey (keyEvent);
            }
            return true;
        }
    }
}