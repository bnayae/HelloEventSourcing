using EventSourcing.Backbone;
using EventSourcing.Demo;

Console.WriteLine("Consuming Events / Reporting");

IConsumerLifetime subscription = RedisConsumerBuilder.Create()
                                            .AddS3Storage(// <- this one set the S3 as the event-source storage
                                                    Constants.S3Options)
                                            .Uri(Constants.URI)
                                            .Group("sample-hello-world:report")
                                            .SubscribeShipmentTrackingConsumer(Subscription.Instance);

await subscription.Completion;
