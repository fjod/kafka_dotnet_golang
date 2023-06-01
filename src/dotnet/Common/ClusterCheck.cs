using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Common;

public class ClusterCheck
{
    public bool CheckIfClusterIsAvailable(IConfiguration configuration)
    {
        using var adminClient = new AdminClientBuilder(configuration.AsEnumerable()).Build();
        bool cluserIsHere = false;
        for (int i = 0; i < 30; i++)
        {
            cluserIsHere = Check(adminClient);
            if (cluserIsHere) break;
            Thread.Sleep(1000);
        }

        if (!cluserIsHere)
        {
            Console.WriteLine("Can't connect to cluster in 60 s, shutting down..");
            return false;
        }

        return true;
    }
    private bool Check(IAdminClient client)
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
}