using Microsoft.Extensions.Options;

namespace Playground.Configuration.Validation;

public class StartupOptionsValidation<T> : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            var options = builder.ApplicationServices.GetRequiredService(typeof(IOptions<>).MakeGenericType(typeof(T)));
            if (options != null!)
            {
                _ = ((IOptions<object>)options).Value;
            }

            next(builder);
        };
    }
}