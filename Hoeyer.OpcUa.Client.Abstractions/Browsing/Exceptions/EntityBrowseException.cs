using System;

namespace Hoeyer.OpcUa.Client.Abstractions.Browsing.Exceptions;

public class EntityBrowseException(string message) : Exception(message);