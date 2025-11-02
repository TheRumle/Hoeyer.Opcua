using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hoeyer.OpcUa.Core.Configuration.Options;

public static class EagerValidationExtensions
{
    public static OptionsBuilder<TOptions> ValidateBeforeStart<TOptions>(this OptionsBuilder<TOptions> optionsBuilder)
        where TOptions : class, new()
    {
        optionsBuilder.Services.AddTransient<IStartupFilter, StartupOptionsValidation<TOptions>>();
        return optionsBuilder;
    }
}