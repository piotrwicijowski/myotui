using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using myotui;
using myotui.Models;
using myotui.Models.Config;

namespace myotui.Services
{
    public class CliRawContentService : IRawContentService
    {
        protected readonly IParameterService _parameterService;
        public CliRawContentService(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }
        public dynamic GetRawOutput(ViewNode node, IDictionary<string, string> parameters)
        {
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            var cliContent = node.Buffer.Content as CliValueContent;
            var cliSubstituted = _parameterService.SubstituteParameters(string.Join(Environment.NewLine,cliContent.Input),parameters);
            var filename = "";
            var args = "";
            if(isWindows)
            {
                filename = "cmd.exe";
                args = "/c " + cliSubstituted;
            }
            else
            {
                var commandSplit = StringExtensions.SplitCommandLine(cliSubstituted);
                filename = commandSplit.Take(1).FirstOrDefault();
                args = string.Join(' ', commandSplit.Skip(1));
            }
            using var pProcess = new Process();
            pProcess.StartInfo.FileName = filename;
            pProcess.StartInfo.Arguments = args;
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