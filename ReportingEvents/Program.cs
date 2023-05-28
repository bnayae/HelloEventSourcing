using EventSourcing.Backbone;
using EventSourcing.Demo;

Console.WriteLine("Consuming Events / Reporting");

IConsumerLifetime subscription = RedisConsumerBuilder.Create()
                                            .Uri(URIs.Default)
                                            .Group("sample-hello-world:report")
                                            .SubscribeShipmentTrackingConsumer(Subscription.Instance);

await subscription.Completion;
