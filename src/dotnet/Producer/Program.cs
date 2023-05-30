// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Producer;
using Producer.StudyTypes;

Console.WriteLine("Producer starting...");

var configuration = Settings.Get();

Stopwatch sw = new Stopwatch();
sw.Start();

var study = TestingFactory.Get(StudyType.WithCustomPartitioner);
await study(configuration);

Console.WriteLine($"Producer finished in {sw.ElapsedMilliseconds} ms");