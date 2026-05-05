using System;
using System.Diagnostics.CodeAnalysis;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Shared.Serialisation;

namespace MessagingService.Bootstrapper;

[ExcludeFromCodeCoverage]
public class SerialiserRegistry : ServiceRegistry
{
    public SerialiserRegistry()
    {
        this.AddSingleton<IStringSerialiser, SystemTextJsonSerializer>();
        this.AddSingleton<Func<Object, String>>(_ => obj => StringSerialiser.Serialise(obj));
        this.AddSingleton<Func<String, Type, Object>>(_ => (str, type) => StringSerialiser.DeserializeObject<Object>(str, type));
        this.AddSingleton(SystemTextJsonSerializer.GetDefaultJsonSerializerOptions());
    }
}