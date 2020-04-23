using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            var cliContent = node.Buffer.Content as CliValueContent;
            var cliSubstituted = _parameterService.SubstituteParameters(string.Join(Environment.NewLine,cliContent.Input),parameters);
            var commandSplit = StringExtensions.SplitCommandLine(cliSubstituted);
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