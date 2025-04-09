using System;

namespace Hoeyer.OpcUa.Client.Application.Reading;

public class NodeReadException(string message) : Exception(message);