﻿using EventSourcing.Backbone;
using EventSourcing.Demo;

class Subscription : IShipmentTrackingConsumer
{
    public static readonly Subscription Instance = new Subscription();
    private readonly IShipmentTrackingProducer _producer = RedisProducerBuilder.Create()
                                .Uri(URIs.Default)
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
        //await Ack.Current.AckAsync(); // acknowledge the message as successfully handled (will start processing next message). It introduce parallelism in a risk of losing the message on unexpected failure
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
        //await Ack.Current.AckAsync(); // acknowledge the message as successfully handled (will start processing next message). It introduce parallelism in a risk of losing the message on unexpected failure
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
        //await Ack.Current.AckAsync(); // acknowledge the message as successfully handled (will start processing next message). It introduce parallelism in a risk of losing the message on unexpected failure
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
