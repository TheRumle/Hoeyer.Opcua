using System;
using System.Reflection;

namespace Hoeyer.OpcUa;

public class OptionsNotConfiguredException: Exception
{
    public OptionsNotConfiguredException(Type optionValueType, Assembly assembly):base($"An option was not registered before trying to call {assembly.GetName()}. The Options pattern must be used to configure an {optionValueType.FullName} as an option, as it is used to configure an OpcUaEntityServer and related services. \n\n You can read more about the options pattern at https://learn.microsoft.com/en-us/dotnet/core/extensions/options")
    {
    }
}