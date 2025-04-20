using System;

namespace Hoeyer.OpcUa.Client.Api.Browsing;

public class EntityBrowseException(string message) : Exception(message);