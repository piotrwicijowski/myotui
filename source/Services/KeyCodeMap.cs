using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terminal.Gui;

namespace myotui.Services
{
    public class KeyCodeMap : IKeyCodeMap
    {
        protected readonly IDictionary<string,Key> _linuxLogicalToPhysicalMaps = new Dictionary<string,Key>()
        {
            {"Tab", Key.ControlI},
            {"Enter", Key.ControlM},
            {"ControlH", Key.Backspace},
        };

        protected readonly IDictionary<string,Key> _windowsLogicalToPhysicalMaps = new Dictionary<string,Key>()
        {
            {"Enter", Key.ControlJ},
        };

        public Key GetPhysicalKeyCode(string logicalKeyName)
        {
            Key result;
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            bool isLinux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            if(isWindows)
            {
                if(_windowsLogicalToPhysicalMaps.ContainsKey(logicalKeyName))
                {
                    return _windowsLogicalToPhysicalMaps[logicalKeyName];
                }
            }
            else
            if(isLinux)
            {
                if(_linuxLogicalToPhysicalMaps.ContainsKey(logicalKeyName))
                {
                    return _linuxLogicalToPhysicalMaps[logicalKeyName];
                }
            }
            var isCorrect = Enum.TryParse<Key>(logicalKeyName, out result);
            if(!isCorrect)
            {
                result = (Key)logicalKeyName[0];
            }
            return result;
        }
         
    }
}