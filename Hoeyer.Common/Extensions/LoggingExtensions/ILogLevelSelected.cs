using JetBrains.Annotations;

namespace Hoeyer.Common.Extensions.LoggingExtensions;

public interface ILogLevelSelected : IFinishedLoggingSetup
{
    [Pure]
    IMessageSelected WithErrorMessage([StructuredMessageTemplate] string message, params object[] messageArguments);
    
    [Pure]
    IScopeSelected WithScope([StructuredMessageTemplate] string scopeTitle, params object[] scopeArguments);
}