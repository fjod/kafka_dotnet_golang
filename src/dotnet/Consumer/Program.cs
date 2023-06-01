// See https://aka.ms/new-console-template for more information

using Common;
using Consumer.Messaging;

Console.WriteLine("Consumer starting..");
string firstTopicName = "first.messages";

var configuration = Settings.Get(false);
ClusterCheck clusterCheck = new ClusterCheck();
if (!clusterCheck.CheckIfClusterIsAvailable(configuration)) return;

CancellationTokenSource cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // prevent the process from terminating.
    cts.Cancel();
};

var consumer = new SimpleConsume();
consumer.Perform(configuration, cts, firstTopicName);