using EventsAbstractions;

using EventSourcing.Backbone;

Console.WriteLine("Producing events");

IHelloEventsProducer producer = RedisProducerBuilder.Create()
                                .Uri(URIs.Default)
                                .BuildHelloEventsProducer();



Console.Write("What is your name? ");
string name = Console.ReadLine();
await producer.NameAsync(name ?? "Unknown");

var rnd = new Random(Guid.NewGuid().GetHashCode());
Console.WriteLine("Press Esc to exit");
Console.Write("Press Number for delay: ");
Console.WriteLine("1 is ms, 2 is 20 ms, 3 is 300 ms, 4 is 4s, 5 is 5s, 6 is 6s");


var colors = Enum.GetValues<ConsoleColor>() ?? Array.Empty<ConsoleColor>();
while (!Console.KeyAvailable || Console.ReadKey(true).Key == ConsoleKey.Escape)
{
    int index = Environment.TickCount % colors.Length;
    var color = colors[index];
    await producer.ColorAcync(color);
    await producer.StarAsync();

    ConsoleKey press = Console.KeyAvailable ? Console.ReadKey(true).Key : ConsoleKey.Clear;
    int delay = press switch
    {
        ConsoleKey.D1 => 1,
        ConsoleKey.D2 => 20,
        ConsoleKey.D3 => 300,
        ConsoleKey.D4 => 4000,
        ConsoleKey.D5 => 5000,
        ConsoleKey.D6 => 6000,
        ConsoleKey.D7 => 7000,
        ConsoleKey.D8 => 8000,
        ConsoleKey.D9 => 9000,
        _ => 0
    };
    if (delay != 0)
        await Task.Delay(delay);
    Console.ForegroundColor = color;
    Console.Write("☆");
}

Console.WriteLine(" Done");