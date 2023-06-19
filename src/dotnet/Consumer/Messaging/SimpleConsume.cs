using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Consumer.Messaging;

public class SimpleConsume
{
    public void Perform(IConfiguration configuration, string topicName)
    {
        using var consumer = new ConsumerBuilder<string, string>(configuration.AsEnumerable()).Build();
        consumer.Subscribe(topicName);
        while (true)
        {
            try
            {

                var cr = consumer.Consume(TimeSpan.FromMilliseconds(100));

                if (cr != null)
                {
                    Console.WriteLine(
                        $"Consumed event from topic {topicName} with key {cr.Message.Key,-10} and value {cr.Message.Value}. Offset committed {cr.Offset}. Partition is :{cr.TopicPartition}");
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