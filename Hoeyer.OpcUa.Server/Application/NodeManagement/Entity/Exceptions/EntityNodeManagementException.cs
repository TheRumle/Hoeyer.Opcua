using System;

namespace Hoeyer.OpcUa.Server.Application.NodeManagement.Entity.Exceptions;

public abstract class EntityNodeManagementException(string message) : Exception(message);