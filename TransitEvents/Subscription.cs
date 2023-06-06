using EventSourcing.Backbone;
using EventSourcing.Demo;

class Subscription : IShipmentTrackingConsumer
{
    public static readonly Subscription Instance = new Subscription();
    private readonly IShipmentTrackingProducer _producer = RedisProducerBuilder.Create()
                                .AddS3Storage() // <- this one set the S3 as the event-source storage
                                .Uri(Constants.URI)
                                .BuildShipmentTrackingProducer();
    private readonly Random _rnd = new Random();

    private async Task Delay()
    {
        var sec = _rnd.Next(1, 4);
        await Task.Delay(sec * 1000);
    }
    public async ValueTask OrderPlacedAsync(
        ConsumerMetadata consumerMetadata,
        User user,
        Product product,
        DateTimeOffset time)
    {
        await Console.Out.WriteLineAsync($"{user.name} order a {product.name}, [product id = {product.id}]");
        await Delay();
        await _producer.PackingAsync(user.email, product.id, DateTimeOffset.Now);
    }

    public async ValueTask PackingAsync(
        ConsumerMetadata consumerMetadata,
        string email,
        int productId,
        DateTimeOffset time)
    {
        await Console.Out.WriteLineAsync($"Packing [product id = {productId}] for {email}");
        await Delay();
        await _producer.OnDeliveryAsync(email, productId, DateTimeOffset.Now);
    }

    public async ValueTask OnDeliveryAsync(
        ConsumerMetadata consumerMetadata,
        string email,
        int productId,
        DateTimeOffset time)
    {
        await Console.Out.WriteLineAsync($"Delivery of [product id = {productId}] for {email}");
        await Delay();
        await _producer.OnReceivedAsync(email, productId, DateTimeOffset.Now);
    }

    public async ValueTask OnReceivedAsync(
        ConsumerMetadata consumerMetadata,
        string email,
        int productId,
        DateTimeOffset time)
    {
        await Console.Out.WriteLineAsync($"[product id = {productId}] for {email} has received");
    }
}
