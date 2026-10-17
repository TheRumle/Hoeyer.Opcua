using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Abstractions;

public static class StatusCodeExtensions
{
    public static bool IsBad(this ServiceResult result) => StatusCode.IsBad(result.StatusCode);
    public static bool IsGood(this ServiceResult result) => StatusCode.IsGood(result.StatusCode);
    public static bool IsNotGood(this ServiceResult result) => StatusCode.IsNotGood(result.StatusCode);
}