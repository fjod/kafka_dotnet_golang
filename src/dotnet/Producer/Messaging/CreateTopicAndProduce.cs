using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;

namespace Producer.Messaging;

/// <summary>
/// Creates new topic and produces 10 default messages there
/// </summary>
public class CreateTopicAndProduce : SendMessages
{
    public async Task Perform(IConfiguration configuration, string topicName)
    {
        using var producer = new ProducerBuilder<string, string>(configuration.AsEnumerable()).Build();

        using var adminClient = new AdminClientBuilder(configuration.AsEnumerable()).Build();
        var topic = new TopicSpecification
        {
            Name = topicName,
            NumPartitions = 2,
            ReplicationFactor = 1
        };

        try
        {
            await adminClient.CreateTopicsAsync(new[] { topic });
            Console.WriteLine($"Topic {topic.Name} created.");
        }
        catch (CreateTopicsException e)
        {
            Console.WriteLine($"An error occurred creating topic {topic.Name}: {e.Message}");
        }
        
        await Produce(producer, topicName);
    }
}