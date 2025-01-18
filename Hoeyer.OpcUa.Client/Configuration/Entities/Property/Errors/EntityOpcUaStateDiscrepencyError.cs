using FluentResults;

namespace Hoeyer.OpcUa.Client.Configuration.Entities.Property.Errors;

public class EntityOpcUaStateDiscrepencyError(string s) : Error(s);