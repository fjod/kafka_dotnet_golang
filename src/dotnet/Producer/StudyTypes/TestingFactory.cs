using Microsoft.Extensions.Configuration;
using Producer.Messaging;

namespace Producer.StudyTypes;

public delegate Task StudyWork(IConfiguration configuration);

public static class TestingFactory
{
    public static StudyWork Get(StudyType input)
    {
        return input switch
        {
            StudyType.SimpleProduce => async configuration =>
            {
                var existing = new SimpleProduce();
                await existing.Perform(configuration);
            },
            StudyType.WithNewTopic => async configuration =>
            {
                var newTopic = new CreateTopicAndProduce();
                await newTopic.Perform(configuration, "testTopic");
            },
            StudyType.WithCustomPartitioner => async configuration =>
            {
                var custom = new CustomPartitioner();
                await custom.Perform(configuration);
            },
            StudyType.SimpleProduceWithParameters => async configuration =>
            {
                var withParams = new ProduceWithParameters();
                await withParams.Perform(configuration);
            },
            _ => throw new ArgumentOutOfRangeException(nameof(input), input, "no delegate for such input")
        };
    }
}