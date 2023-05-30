using Confluent.Kafka;

namespace Producer.Messaging;

public abstract class SendMessages
{
    protected string firstTopicName = "first.messages";
    protected readonly string[] Users = {"eabara", "jsmith", "sgarcia", "jbernard", "htanaka", "awalther"};
    private readonly string[] _items = {"book", "alarm clock", "t-shirts", "gift card", "batteries"};
    private readonly Random _rnd = new Random();
    protected async Task Produce(IProducer<string,string> producer, string topic)
    {
        var numProduced = 0;
       
        const int numMessages = 10;
        for (int i = 0; i < numMessages; ++i)
        {
            var user = Users[_rnd.Next(Users.Length)];
            var item = _items[_rnd.Next(_items.Length)];

            try
            {
                var result = await producer.ProduceAsync(topic, new Message<string, string> {Key = user, Value = item}, CancellationToken.None);
                Console.WriteLine($"Produced event to topic {result.Topic}: key = {user,-10} value = {item}, msg key = {result.Message.Key}, msg value = {result.Message.Value}");
                numProduced += 1;
            }
            catch (ProduceException<string, string> e)
            {
                Console.WriteLine($"Failed to deliver message: {e.Error.Reason}");
            }
        }

        producer.Flush(TimeSpan.FromSeconds(10));
        Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
    }
}