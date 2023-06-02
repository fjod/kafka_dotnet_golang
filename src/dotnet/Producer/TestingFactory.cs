using Common;
using Common.StudyTypes;
using Microsoft.Extensions.Configuration;
using Producer.Messaging;

namespace Producer;

public static class TestingFactory
{
    public static StudyWork Get(ProducerStudyType input)
    {
        return input switch
        {
            ProducerStudyType.SimpleProduce => async configuration =>
            {
                var existing = new SimpleProduce();
                await existing.Perform(configuration);
            },
            ProducerStudyType.WithNewTopic => async configuration =>
            {
                var newTopic = new CreateTopicAndProduce();
                await newTopic.Perform(configuration, "testTopic");
            },
            ProducerStudyType.WithCustomPartitioner => async configuration =>
            {
                var custom = new CustomPartitioner();
                await custom.Perform(configuration);
            },
            ProducerStudyType.SimpleProduceWithParameters => async configuration =>
            {
                var withParams = new ProduceWithParameters();
                await withParams.Perform(configuration);
            },
            _ => throw new ArgumentOutOfRangeException(nameof(input), input, "no delegate for such input")
        };
    }
}