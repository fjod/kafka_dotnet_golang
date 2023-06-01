using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Consumer.Messaging;

public class SimpleConsume
{
    public void Perform(IConfiguration configuration, CancellationTokenSource cts, string topicName)
    {
        using var consumer = new ConsumerBuilder<string, string>(configuration.AsEnumerable()).Build();
        consumer.Subscribe(topicName);
        try
        {
            while (true)
            {
                var cr = consumer.Consume(TimeSpan.FromMilliseconds(1000));
             
                if (cr != null)
                {
                    Console.WriteLine(
                        $"Consumed event from topic {topicName} with key {cr.Message.Key,-10} and value {cr.Message.Value}. Offset committed {cr.Offset}");
                    consumer.Commit(cr);
                }
                if (cts.Token.IsCancellationRequested)break;
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