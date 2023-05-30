using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Producer.Messaging;

/// <summary>
/// docker-compose creates topics:
/// 9092 : "first.messages"
/// 9093 : "second.messages" , "second.users"
/// </summary>
public class ProduceToExistingTopic : SendMessages
{
    public async Task Perform(IConfiguration configuration)
    {
        using var producer = new ProducerBuilder<string, string>(configuration.AsEnumerable()).Build();
        await Produce(producer, firstTopicName);
    }
}