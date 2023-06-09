using EventSourcing.Backbone;
using EventSourcing.Demo;

Console.WriteLine("Consuming and Producing Events");

IConsumerLifetime subscription = RedisConsumerBuilder.Create()
                                            .AddS3Storage(// <- this one set the S3 as the event-source storage
                                                    Constants.S3Options)
                                            //,
                                            //envAccessKey: Constants.S3_ACCESS_KEY_ENV,
                                            //envSecretKey: Constants.S3_SECRET_ENV) 
                                            .Uri(Constants.URI)
                                            //.Group("sample-hello-world")
                                            .SubscribeShipmentTrackingConsumer(Subscription.Instance);

await subscription.Completion;
