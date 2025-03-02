using JetBrains.Annotations;

namespace Hoeyer.Common.Extensions.LoggingExtensions;

public interface IScopeSelected : IFinishedLoggingSetup
{
    [Pure]
    IScopeAndMessageSelected WithErrorMessage([StructuredMessageTemplate] string message,
        params object[] messageArguments);
}