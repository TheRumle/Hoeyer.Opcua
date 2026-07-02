using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Abstractions.Browsing.Exceptions;

public class InvalidBrowseRootException(NodeId id)
    : EntityBrowseException($"No node with id {id.ToString()} could be found!");