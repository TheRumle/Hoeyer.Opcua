using System;

namespace Hoeyer.OpcUa.Client.Api.Browsing.Exceptions;

public class AgentBrowseException(string message) : Exception(message);