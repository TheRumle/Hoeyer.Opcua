using System;

namespace Hoeyer.OpcUa.Core.Api.NodeStructure;

public sealed class ModellingMismatchException(string message)
    : Exception("An entity modelling mismatch occured: " + message);