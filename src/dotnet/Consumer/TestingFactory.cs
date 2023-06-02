using Common;
using Common.StudyTypes;
using Consumer.Messaging;

namespace Consumer;

public static class TestingFactory
{
    private const string FirstTopicName = "first.messages";

    public static StudyWork Get(ConsumerStudyType input)
    {
        return input switch
        {
            ConsumerStudyType.SimpleConsume => configuration =>
            {
                var sc = new SimpleConsume();
                sc.Perform(configuration, FirstTopicName);
                return Task.CompletedTask;
            },
            ConsumerStudyType.UsingDataflow => async configuration =>
            {
                var df = new Dataflow();
                await df.Perform(configuration, FirstTopicName);
            },
            
            ConsumerStudyType.ConsumeFromTopic => configuration =>
            {
                var cft = new ConsumeFromTopic();
                cft.Perform(configuration, FirstTopicName);
                return Task.CompletedTask;
            },
            _ => throw new ArgumentOutOfRangeException(nameof(input), input, "no delegate for such input")
        };
    }
}