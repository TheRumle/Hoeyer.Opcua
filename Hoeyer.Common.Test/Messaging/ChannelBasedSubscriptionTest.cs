using Hoeyer.Common.Messaging.Subscriptions.ChannelBased;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging.Abstractions;

namespace Hoeyer.Common.Test.Messaging;

[InheritsTests]
[TestSubject(typeof(ChannelBasedSubscription<>))]
public class ChannelBasedSubscriptionTest() : SubscriptionSystemTest(
    new ChannelSubscriptionFactory<int>(NullLoggerFactory.Instance));