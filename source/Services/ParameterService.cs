using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace myotui.Services
{
    public class ParameterService : IParameterService
    {
        public string SubstituteParameters(string input, IDictionary<string, string> parameters)
        {
            return parameters.ToList().Aggregate(input, (acc, pair) =>
            {
                if(string.IsNullOrEmpty(pair.Value))
                {
                    acc = Regex.Replace(acc,$"\\?\\({pair.Key}:([^\\)]+)\\)","");
                }
                else
                {
                    acc = Regex.Replace(acc,$"\\?\\({pair.Key}:([^\\)]+)\\)","$1");
                }
                return acc.Replace($"%({pair.Key})",pair.Value);
            }
            );
        }

        public IDictionary<string, string> DecodeParametersString(string parameters, IList<string> parameterNames)
        {
            var results = new Dictionary<string, string>();
            if(parameterNames == null || !parameterNames.Any() || string.IsNullOrEmpty(parameters))
            {
                return results;
            }
            var unusedParameters = new List<string>(parameterNames);
            var unusedValues = new List<string>();
            parameters.Split(' ',StringSplitOptions.RemoveEmptyEntries)
                .ToList().ForEach(parameter =>
                {
                    var keyValuePair = parameter.Split(':');
                    var value = keyValuePair.Last();
                    if(keyValuePair.Count() > 1)
                    {
                        var key = keyValuePair.First();
                        unusedParameters.RemoveAll(param => param == key);
                        results.Add(key, value);
                    }
                    else
                    {
                        unusedValues.Add(value);
                    }
                }
            );
            unusedParameters.Zip(unusedValues, (key, value) => (key, value)).ToList()
                .ForEach(
                    pair =>
                    {
                        (string key, string value) = pair;
                        results.Add(key, value);
                    }
                );
            return results;
        }
    }
}