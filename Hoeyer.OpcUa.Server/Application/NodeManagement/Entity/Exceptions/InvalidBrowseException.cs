namespace Hoeyer.OpcUa.Server.Application.NodeManagement.Entity.Exceptions;

internal class InvalidBrowseException(string message) : EntityNodeManagementException(message);