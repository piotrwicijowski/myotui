using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myotui;

namespace myotui.Services
{
    public class CliRawContentService : IRawContentService
    {
        public string GetRawOutput(string command)
        {
            var commandSplit = StringExtensions.SplitCommandLine(command);
            using var pProcess = new Process();
            pProcess.StartInfo.FileName = commandSplit.Take(1).FirstOrDefault();
            pProcess.StartInfo.Arguments = string.Join(' ', commandSplit.Skip(1));
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
            pProcess.Start();
            string output = pProcess.StandardOutput.ReadToEnd(); //The output result
            pProcess.WaitForExit();
            return output;
        }

    }
}