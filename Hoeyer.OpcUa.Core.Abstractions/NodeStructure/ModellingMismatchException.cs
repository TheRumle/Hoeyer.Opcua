namespace Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

public sealed class ModellingMismatchException(string message)
    : Exception("An entity modelling mismatch occured: " + message);