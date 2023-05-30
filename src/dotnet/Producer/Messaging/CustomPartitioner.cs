using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Producer.Messaging;

/// <summary>
/// Send first 3 users to partition 0, second 3 users to partition 1
/// </summary>
public class CustomPartitioner : SendMessages
{
    public async Task Perform(IConfiguration configuration)
    {
        var producerBuilder = new ProducerBuilder<string, string>(configuration.AsEnumerable());
        producerBuilder.SetDefaultPartitioner((_, _, keyData, keyIsNull) => keyIsNull ? 0 : KeyForUser(keyData));
        using var producer = producerBuilder.Build();
        await Produce(producer, firstTopicName);
    }

    private int KeyForUser(ReadOnlySpan<byte> key)
    {
        var keyString = System.Text.Encoding.UTF8.GetString(key.ToArray());
        var keyIndex = -1;
        for (int i = 0; i < Users.Length; i++)
        {
            if (keyString != Users[i]) continue;
            keyIndex = i;
            break;
        }

        return keyIndex switch
        {
            <3 => 0,
            _ => 1
        };
    }
}