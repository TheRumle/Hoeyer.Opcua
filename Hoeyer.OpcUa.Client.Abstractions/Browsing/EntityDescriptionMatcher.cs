using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Abstractions.Browsing;

public delegate bool EntityDescriptionMatcher<TEntity>(ReferenceDescription reference);