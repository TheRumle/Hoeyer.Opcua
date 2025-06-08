using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Extensions;

public static class StatusCodeExtensions
{
    public static bool IsBad(this ServiceResult result) => StatusCode.IsBad(result.StatusCode);
    public static bool IsGood(this ServiceResult result) => StatusCode.IsGood(result.StatusCode);
    public static bool IsNotGood(this ServiceResult result) => StatusCode.IsNotGood(result.StatusCode);
    public static bool IsNotBad(this ServiceResult result) => StatusCode.IsNotBad(result.StatusCode);
    public static bool IsUncertain(this ServiceResult result) => StatusCode.IsUncertain(result.StatusCode);
    public static string Name(this ServiceResult result) => result.StatusCode.Name();
    public static string Name(this StatusCode result) => StatusCodes.GetBrowseName(result.Code);
    public static string GetStatusCodeName(uint result) => new StatusCode(result).Name();
}