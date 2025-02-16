using System;

namespace Hoeyer.OpcUa.Server.Application.Node.Entity.Exceptions;

public abstract class EntityNodeManagementException(string message) : InvalidOperationException(message);