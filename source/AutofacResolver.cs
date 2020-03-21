using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Autofac.Extensions.DependencyInjection;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

public class AutofacResolver : INodeTypeResolver
{
    private readonly AutofacServiceProvider _provider;
    public AutofacResolver(AutofacServiceProvider provider){
        _provider = provider;
    }
    public bool Resolve(NodeEvent nodeEvent, ref Type currentType)
    {
        var registrations = _provider.GetAutofacRoot().ComponentRegistry
            .RegistrationsFor(new TypedService(currentType));

        if (registrations.Any())
        {
            var registration = registrations.FirstOrDefault();
            if (!string.IsNullOrEmpty(nodeEvent.Tag))
            {
                var possibleName = $"{nodeEvent.Tag.TrimStart('!')}{currentType.Name.Substring(1)}";
                registration = registrations.FirstOrDefault(x => x.Activator.LimitType.Name.ToLower() == possibleName.ToLower());
            }
            var activator = registration.Activator as ReflectionActivator;
            if (activator != null)
            {
                //we can get the type
                currentType = activator.LimitType;
                return true;
            }
        }

        return false;
    }
}