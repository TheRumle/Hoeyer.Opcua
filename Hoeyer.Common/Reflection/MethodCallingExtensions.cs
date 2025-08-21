using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.Common.Reflection;

public static class MethodCallingExtensions
{
    public static MethodInfo GetGenericStaticMethod(this object o, Type genericArg, string method)
        => o.GetType().GetMethod(nameof(method), BindingFlags.NonPublic | BindingFlags.Static)
            .MakeGenericMethod(genericArg);

    public static MethodInfo GetGenericStaticMethod(this Type o, Type genericArg, string method)
        => o.GetMethod(nameof(method), BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(genericArg);

    public static MethodInvoker InvokeStaticEntityRegistration(this Type t, string name, IServiceCollection collection)
    {
        var m = t.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
        return new MethodInvoker(m, collection);
    }


    public class MethodInvoker(MethodInfo rawMethod, IServiceCollection collection)
    {
        public void Invoke(Type entity)
        {
            rawMethod.MakeGenericMethod(entity).Invoke(null, new object[] { collection });
        }
    }
}