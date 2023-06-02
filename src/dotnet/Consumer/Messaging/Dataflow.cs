using System.Threading.Tasks.Dataflow;
using Confluent.Kafka;
using Confluent.Kafka.Dataflow;
using Microsoft.Extensions.Configuration;

namespace Consumer.Messaging;

public class Dataflow
{
    public async Task Perform(IConfiguration configuration, string topicName)
    {
        using var consumer = new ConsumerBuilder<string, string>(configuration.AsEnumerable()).Build();
        consumer.Subscribe(topicName);
        
        var processor = new TransformBlock<Message<string, string>, Message<string, string>>(message =>
            {
                Console.WriteLine(
                    $"Consumed event from topic {topicName} with key {message.Key,-10} and value {message.Value}");
                return Task.FromResult(message);
            },
            new ExecutionDataflowBlockOptions
            {
                // Parallelism is OK as long as order is preserved.
                MaxDegreeOfParallelism = 8,
                EnsureOrdered = true,
            });

        // Initialize source and link to target.
        var blockOptions = new DataflowBlockOptions
        {
            // It's a good idea to limit buffered messages (in case processing falls behind).
            // Otherwise, all messages are offered as soon as they are available.
            BoundedCapacity = 8,
        };

        var source = consumer.AsSourceBlock(out var commitTarget, options: blockOptions);
        source.LinkTo(processor, new DataflowLinkOptions { PropagateCompletion = true });
        processor.LinkTo(commitTarget, new DataflowLinkOptions { PropagateCompletion = true });

        // Wait for processing to finish.
        await processor.Completion;
        consumer.Close();
    }
}