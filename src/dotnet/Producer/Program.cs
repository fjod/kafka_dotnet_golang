// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Producer;
using Producer.StudyTypes;

Console.WriteLine("Producer starting..");

var configuration = Settings.Get();

using var adminClient = new AdminClientBuilder(configuration.AsEnumerable()).Build();
bool cluserIsHere = false;
for (int i = 0; i < 30; i++)
{
    cluserIsHere = CheckIfClusterIsAvailable(adminClient);
    if (cluserIsHere) break;
    Thread.Sleep(1000);
}

if (!cluserIsHere)
{
    Console.WriteLine("Can't connect to cluster in 60 s, shutting down..");
    return;
}

Stopwatch sw = new Stopwatch();
sw.Start();

var studyType = Settings.GetStudyType();
Console.WriteLine(studyType != null ? $"Found env var studyType =  {studyType}" : "No env var studyType found, using SimpleProduce");

var study = TestingFactory.Get(studyType ?? StudyType.SimpleProduce);
await study(configuration);

Console.WriteLine($"Producer finished in {sw.ElapsedMilliseconds} ms");

bool CheckIfClusterIsAvailable(IAdminClient client)
{
    try
    {
        var metadata = client.GetMetadata(TimeSpan.FromSeconds(1));
        Console.WriteLine($"Cluster has {metadata.Brokers.Count} brokers:");
        foreach (var broker in metadata.Brokers)
        {
            Console.WriteLine($" Broker {broker.BrokerId} at {broker.Host}:{broker.Port}");
        }

        return true;
    }
    catch (Exception e)
    {
        Console.WriteLine($"Failed to connect: {e.Message}");
    }

    return false;
}