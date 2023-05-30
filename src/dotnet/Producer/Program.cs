// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Producer;
using Producer.Messaging;

async Task Test1(IConfiguration configuration)
{
    var existing = new ProduceToExistingTopic();
    await existing.Perform(configuration);
}

async Task Test2(IConfiguration configuration)
{
    var newTopic = new CreateTopicAndProduce();
    await newTopic.Perform(configuration, "testTopic");
}

Console.WriteLine("Producer starting...");

var configuration = Settings.Get();

Stopwatch sw = new Stopwatch();
sw.Start();

//await Test1(configuration);
//await Test2(configuration);

Console.WriteLine($"Producer finished in {sw.ElapsedMilliseconds} ms");