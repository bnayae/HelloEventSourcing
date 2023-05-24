using EventsAbstractions;

using EventSourcing.Backbone;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Drawing;

Console.WriteLine("Interactive Events");

var enumerator = RedisConsumerBuilder.Create()
                        .Uri(URIs.Default)
                        .BuildIterator()
                        .SpecializeHelloEventsConsumer()
                        .GetAsyncEnumerable<HelloEvents_Star>(
                            new ConsumerAsyncEnumerableOptions { ExitWhenEmpty = false });
await foreach (var star in enumerator)
{ 
        Console.Write("✱");
}
                                            
              