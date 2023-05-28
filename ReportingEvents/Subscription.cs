using EventSourcing.Demo;

class Subscription : IShipmentTrackingConsumer
{
    public static readonly Subscription Instance = new Subscription();

    public async ValueTask OrderPlacedAsync(User user, Product product, DateTimeOffset time)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        await Console.Out.WriteLineAsync($"## {user.name} order a {product.name}, [product id = {product.id}]");
    }

    public async ValueTask PackingAsync(string email, int productId, DateTimeOffset time)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        await Console.Out.WriteLineAsync($"## Packing [product id = {productId}] for {email}");
    }

    public async ValueTask OnDeliveryAsync(string email, int productId, DateTimeOffset time)
    {
        Console.ForegroundColor = ConsoleColor.White;
        await Console.Out.WriteLineAsync($"## Delivery of [product id = {productId}] for {email}");
    }

    public async ValueTask OnReceivedAsync(string email, int productId, DateTimeOffset time)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        await Console.Out.WriteLineAsync($"## [product id = {productId}] for {email} has received");
    }
}
