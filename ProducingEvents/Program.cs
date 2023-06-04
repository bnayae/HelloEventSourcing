using EventSourcing.Backbone;
using EventSourcing.Demo;



Console.WriteLine("Producing events");

IShipmentTrackingProducer producer = RedisProducerBuilder.Create()
                                .AddS3Strategy( // <- this one set the S3 as the event-source storage
                                        Constants.S3Options)
                                .Uri(Constants.URI)
                                .BuildShipmentTrackingProducer();

Console.Write("What is your name? ");
string name = Console.ReadLine() ?? "someone";
Console.Write("What is your email? ");
string email = Console.ReadLine() ?? "someone@gmail.com";
User user = new(1, email, name);

foreach (ProductList p in Enum.GetValues(typeof(ProductList)))
{
    int price = (int)p * 1000;
    string prodName = p.ToString();
    var product = new Product(prodName.GetHashCode(), prodName, price);
    await producer.OrderPlacedAsync(user, product, DateTimeOffset.UtcNow);
    await Console.Out.WriteLineAsync($"{p} order has placed");
    await Task.Delay(1000);
}


Console.WriteLine("Done");
