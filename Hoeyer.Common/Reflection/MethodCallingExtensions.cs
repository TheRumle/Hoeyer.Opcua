using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.Common.Reflection;

public static class MethodCallingExtensions
{
    public static StaticRegistration CreateStaticMethodInvoker(this Type t, string name, IServiceCollection collection)
    {
        var m = t.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
        return new StaticRegistration(m, collection);
    }

    public static StaticRegistrationWithProvider CreateStaticMethodInvoker(this Type t, string name,
        IServiceCollection collection, IServiceProvider provider)
    {
        var m = t.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
        return new StaticRegistrationWithProvider(m, collection, provider);
    }

    public class StaticRegistrationWithProvider(
        MethodInfo rawMethod,
        IServiceCollection collection,
        IServiceProvider provider)
    {
        public void Invoke(Type entity)
        {
            rawMethod.MakeGenericMethod(entity).Invoke(null, new object[] { collection, provider });
        }
    }

    public class StaticRegistration(MethodInfo rawMethod, IServiceCollection collection)
    {
        public void Invoke(Type entity)
        {
            rawMethod.MakeGenericMethod(entity).Invoke(null, new object[] { collection });
        }
    }
}