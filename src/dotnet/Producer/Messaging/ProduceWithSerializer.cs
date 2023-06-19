using Common;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using DummyProtoc;
using Microsoft.Extensions.Configuration;

namespace Producer.Messaging;

public class ProduceWithSerializer : SendMessages
{
    public async Task Perform(IConfiguration configuration)
    {
        var producerBuilder = new ProducerBuilder<string, Dummy>(configuration.AsEnumerable());
        producerBuilder.SetValueSerializer(new DummySerializer());
        var producer = producerBuilder.Build();

        using var adminClient = new AdminClientBuilder(configuration.AsEnumerable()).Build();
        var topic = new TopicSpecification
        {
            Name = "dummyTopic",
            NumPartitions = 2,
            ReplicationFactor = 1
        };


        var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
        if (metadata.Topics.All(t => t.Topic != "dummyTopic"))
        {
            try
            {
                await adminClient.CreateTopicsAsync(new[] {topic});
                Console.WriteLine($"Topic {topic.Name} created.");
            }
            catch (CreateTopicsException e)
            {
                Console.WriteLine($"An error occurred creating topic {topic.Name}: {e.Message}");
            }
        }

        var numProduced = 0;

        for (int i = 0; i < 10; ++i)
        {
            try
            {
                var dummy = new Dummy
                {
                    Name = Users[_rnd.Next(Users.Length)],
                    Age = i
                };
                var message = new Message<string, Dummy> {Key = dummy.Name, Value = dummy};
                var result = await producer.ProduceAsync("dummyTopic", message);
                Console.WriteLine(
                    $"Produced event to topic {result.Topic}: key = {dummy.Name,-10} value = {dummy.Age}, msg key = {result.Message.Key}, msg value = {result.Message.Value}");
                numProduced += 1;
            }
            catch (ProduceException<string, Dummy> e)
            {
                Console.WriteLine($"Failed to deliver message: {e.Error.Reason}");
            }
        }

        producer.Flush(TimeSpan.FromSeconds(10));
        Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
    }
}