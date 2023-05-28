using EventSourcing.Demo;
using EventSourcing.Backbone;

Console.WriteLine("Consuming and Producing Events");

IConsumerLifetime subscription = RedisConsumerBuilder.Create()
                                            .Uri(URIs.Default)
                                            //.Group("sample-hello-world")
                                            .SubscribeShipmentTrackingConsumer(Subscription.Instance);

await subscription.Completion;
