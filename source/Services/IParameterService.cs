using System.Collections.Generic;

namespace myotui.Services
{
    public interface IParameterService
    {
        public string SubstituteParameters(string input, IDictionary<string, string> parameters);

        public IDictionary<string, string> DecodeParametersString(string parameters, IList<string> parameterNames);
    }
}