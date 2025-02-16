namespace Hoeyer.OpcUa.Server.Application.Node.Entity.Exceptions;

internal class InvalidBrowseException(string message) : EntityNodeManagementException(message);