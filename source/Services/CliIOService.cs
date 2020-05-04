using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace myotui.Services
{
    public class CliIOService : IIOService
    {
        public dynamic Run(dynamic input)
        {
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            var cli = (string)input;
            var filename = "";
            var args = "";
            if(isWindows)
            {
                filename = "cmd.exe";
                args = "/c " + cli;
            }
            else
            {
                var commandSplit = StringExtensions.SplitCommandLine(cli);
                filename = commandSplit.Take(1).FirstOrDefault();
                args = string.Join(' ', commandSplit.Skip(1));
            }
            using var pProcess = new Process();
            pProcess.StartInfo.FileName = filename;
            pProcess.StartInfo.Arguments = args;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            string output = pProcess.StandardOutput.ReadToEnd();
            pProcess.WaitForExit();
            return output;
        }
    }
}