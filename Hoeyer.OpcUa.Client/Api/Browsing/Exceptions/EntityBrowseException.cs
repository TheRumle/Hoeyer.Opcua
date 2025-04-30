using System;

namespace Hoeyer.OpcUa.Client.Api.Browsing.Exceptions;

public class EntityBrowseException(string message) : Exception(message);