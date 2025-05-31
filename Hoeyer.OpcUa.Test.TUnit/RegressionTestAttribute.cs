namespace Hoeyer.Opc.Ua.Test.TUnit;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class RegressionTestAttribute(string againstWhat, params Type[] subjects) : Attribute;