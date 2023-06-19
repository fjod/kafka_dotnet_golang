using Common;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using DummyProtoc;

namespace Consumer.Messaging;

public class ConsumeWithSerializer
{
    public async Task Perform(IConfiguration configuration)
    {
        var consumerBuilder = new ConsumerBuilder<string, Dummy>(configuration.AsEnumerable());
        consumerBuilder.SetValueDeserializer(new DummySerializer());
        using var consumer = consumerBuilder.Build();

        using var adminClient = new AdminClientBuilder(configuration.AsEnumerable()).Build();
        bool topicIsHere = false;
        for (int i = 0; i < 10; i++)
        {
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            if (metadata.Topics.All(t => t.Topic != "dummyTopic"))
            {
                Console.WriteLine("Topic dummyTopic was not found, waiting 1s for producer to create it...");
                await Task.Delay(1000);
            }
            else
            {
                topicIsHere = true;
                break;
            }
        }

        if (!topicIsHere)
        {
            Console.WriteLine("Topic dummyTopic was not found, exiting...");
            return;
        }


        consumer.Subscribe("dummyTopic");

        while (true)
        {
            try
            {
                var cr = consumer.Consume(TimeSpan.FromMilliseconds(100));

                if (cr != null)
                {
                    Console.WriteLine($"Consumed event from dummyTopic with key {cr.Message.Key,-10} and dummyName {cr.Message.Value.Name} + dummy age {cr.Message.Value.Age}.");
                    consumer.Commit(cr);
                }
            }
            catch (OperationCanceledException)
            {
                // Ctrl-C was pressed.
                consumer.Close();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: " + ex.Message);
            }
        }
    }
}