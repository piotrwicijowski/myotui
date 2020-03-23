using System.Text;
using Terminal.Gui;

namespace myotui.Services {
    public class KeyService : IKeyService {
        //TODO this is only for demonstration purposes
        private readonly IActionService _actionService;
        public KeyService (IActionService actionService)
        {
            _actionService = actionService;
        }
        public void ProcessKeyEvent (KeyEvent keyEvent)
        {
            switch(keyEvent.KeyValue)
            {
                case (int)('h'):
                    _actionService.DispatchAction("/body/sidebar.focus","/");
                    break;
                case (int)('l'):
                    _actionService.DispatchAction("/body/main.focus","/");
                    break;
            }

        }
    }
}