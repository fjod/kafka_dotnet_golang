using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Consumer.Messaging;

public class ConsumeFromTopic
{
    public void Perform(IConfiguration configuration, string topicName)
    {
        using var consumer = new ConsumerBuilder<string, string>(configuration.AsEnumerable()).Build();
        var partition = new TopicPartition(topicName, 0); // specify the partition you want to consume from
        consumer.Assign(new[] { partition }); // assign the partition to the consumer
        
        try
        {
            while (true)
            {
                var cr = consumer.Consume(TimeSpan.FromMilliseconds(1000));
             
                if (cr != null)
                {
                    Console.WriteLine(
                        $"Consumed event from topic {topicName} with key {cr.Message.Key,-10} and value {cr.Message.Value}. Offset committed {cr.Offset}. Partition is :{cr.TopicPartition}");
                    consumer.Commit(cr);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Ctrl-C was pressed.
        }
        finally
        {
            consumer.Close();
        }
    }
}