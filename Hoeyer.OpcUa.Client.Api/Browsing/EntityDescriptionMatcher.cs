using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Api.Browsing;

public delegate bool EntityDescriptionMatcher<TEntity>(ReferenceDescription reference);