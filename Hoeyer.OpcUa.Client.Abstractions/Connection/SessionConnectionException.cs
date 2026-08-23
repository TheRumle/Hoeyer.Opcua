using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Abstractions.Connection;

public sealed class SessionConnectionException : Exception
{
    public SessionConnectionException(ServiceResultException ex)
        : base(
            $"{new StatusCode(ex.StatusCode).SymbolicId}: {ex.Message}",
            ex)
    {
        StatusCode = ex.StatusCode;
    }

    public StatusCode StatusCode { get; }
}