using Common;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Producer.Messaging;

public class ProduceWithParameters : SendMessages
{
    public async Task Perform(IConfiguration configuration)
    {
        var produceParams = Settings.GetProduceParameters();
        using var producer = new ProducerBuilder<string, string>(configuration.AsEnumerable()).Build();
        var delay = produceParams.delay ?? 100;
        var amount = produceParams.amount ?? 10;
        for (int i = 0; i < amount; i++)
        {
            Console.WriteLine($"Producing {i}th iteration");
            await Produce(producer, firstTopicName, amount);
            await Task.Delay(delay);
        }
    }
}