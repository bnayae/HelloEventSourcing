using EventSourcing.Demo;
using EventSourcing.Backbone;

Console.WriteLine("Consuming Events / Reporting");

IConsumerLifetime subscription = RedisConsumerBuilder.Create()
                                            .Uri(URIs.Default)
                                            .Group("sample-hello-world:report")
                                            .SubscribeShipmentTrackingConsumer(Subscription.Instance);

Console.ReadKey(false);
