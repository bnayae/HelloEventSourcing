using EventSourcing.Backbone;
using EventSourcing.Demo;

Console.WriteLine("Interactive Events");

var enumerator = RedisConsumerBuilder.Create()
                        .Uri(URIs.Default)
                        .BuildIterator()
                        .SpecializeShipmentTrackingConsumer()
                        .GetAsyncEnumerable<ShipmentTracking_OrderPlaced>(
                            new ConsumerAsyncEnumerableOptions { ExitWhenEmpty = false });
await foreach (ShipmentTracking_OrderPlaced order in enumerator)
{
    Console.WriteLine($"{order.product.name} was order at {order.time}, by {order.user.name}");
}

