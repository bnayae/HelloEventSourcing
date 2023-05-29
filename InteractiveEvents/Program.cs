using EventSourcing.Backbone;
using EventSourcing.Backbone.Building;
using EventSourcing.Demo;

Console.WriteLine("Interactive Events");

var enumerator = RedisConsumerBuilder.Create()
                                .AddS3Strategy(// <- this one set the S3 as the event-source storage
                                        Constants.S3Options)
                                        //,
                                        //envAccessKey: Constants.S3_ACCESS_KEY_ENV,
                                        //envSecretKey: Constants.S3_SECRET_ENV) 
                        .Uri(Constants.URI)
                        .BuildIterator()
                        .SpecializeShipmentTrackingConsumer()
                        .GetAsyncEnumerable<ShipmentTracking_OrderPlaced>(
                            new ConsumerAsyncEnumerableOptions { ExitWhenEmpty = false });
await foreach (ShipmentTracking_OrderPlaced order in enumerator)
{
    Console.WriteLine($"{order.product.name} was order at {order.time}, by {order.user.name}");
}

