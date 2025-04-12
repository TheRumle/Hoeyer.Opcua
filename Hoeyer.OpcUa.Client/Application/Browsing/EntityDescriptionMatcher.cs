using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Application.Browsing;

public delegate bool EntityDescriptionMatcher<TEntity>(ReferenceDescription reference);