using System;
using Common.StudyTypes;
using Xunit.Abstractions;

namespace DotnetTest;

public class EnumsTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public EnumsTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TestProducer()
    {
        var values = Enum.GetValues<ProducerStudyType>();
        foreach (var value in values)
        {
            try
            {
                Producer.TestingFactory.Get(value);
            }
            catch (ArgumentOutOfRangeException)
            {
                _testOutputHelper.WriteLine($"Not found a delegate for {value}");
            }
            catch (Exception e)
            {
                _testOutputHelper.WriteLine($"Exception while creating delegate for {value}, {e.Message}");
            }
        }
    }
    
    [Fact]
    public void TestConsumer()
    {
        var values = Enum.GetValues<ConsumerStudyType>();
        foreach (var value in values)
        {
            try
            {
                Consumer.TestingFactory.Get(value);
            }
            catch (ArgumentOutOfRangeException)
            {
                _testOutputHelper.WriteLine($"Not found a delegate for {value}");
            }
            catch (Exception e)
            {
                _testOutputHelper.WriteLine($"Exception while creating delegate for {value}, {e.Message}");
            }
        }
    }
}