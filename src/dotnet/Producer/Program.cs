// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Common;
using Producer;

Console.WriteLine("Producer starting..");

var configuration = Settings.Get(true);

ClusterCheck clusterCheck = new ClusterCheck();
if (!clusterCheck.CheckIfClusterIsAvailable(configuration)) return;

Stopwatch sw = new Stopwatch();
sw.Start();

var studyType = Settings.GetStudyType();
Console.WriteLine(studyType != null ? $"Found env var studyType =  {studyType}" : "No env var studyType found, using SimpleProduce");

var study = TestingFactory.Get(studyType ?? ProducerStudyType.SimpleProduce);
await study(configuration);

Console.WriteLine($"Producer finished in {sw.ElapsedMilliseconds} ms");
