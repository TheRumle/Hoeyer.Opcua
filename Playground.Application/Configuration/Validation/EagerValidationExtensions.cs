using Microsoft.Extensions.Options;

namespace Playground.Configuration.Validation;

public static class EagerValidationExtensions
{
    public static OptionsBuilder<TOptions> ValidateBeforeStart<TOptions>(this OptionsBuilder<TOptions> optionsBuilder)
        where TOptions : class, new()
    {
        optionsBuilder.Services.AddTransient<IStartupFilter, StartupOptionsValidation<TOptions>>();
        return optionsBuilder;
    }
}