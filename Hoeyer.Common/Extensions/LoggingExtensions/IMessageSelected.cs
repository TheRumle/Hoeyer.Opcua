using JetBrains.Annotations;

namespace Hoeyer.Common.Extensions.LoggingExtensions;

public interface IMessageSelected : IFinishedLoggingSetup
{
    [Pure]
    IScopeAndMessageSelected WithScope([StructuredMessageTemplate] string scopeTitle, params object[] scopeArguments);
}