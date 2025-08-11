using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Api.Browsing;

public delegate bool AgentDescriptionMatcher<TAgent>(ReferenceDescription reference);