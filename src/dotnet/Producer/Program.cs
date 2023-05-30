// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Producer;
using Producer.StudyTypes;

Console.WriteLine("Producer starting...");

var configuration = Settings.Get();

Stopwatch sw = new Stopwatch();
sw.Start();

var studyType = Settings.GetStudyType();
Console.WriteLine(studyType != null ? $"Found env var studyType =  {studyType}" : "No env var studyType found, using SimpleProduce");
Console.WriteLine($"Port is : {configuration.AsEnumerable().First().Value}");

var study = TestingFactory.Get(studyType ?? StudyType.SimpleProduce);
await study(configuration);

Console.WriteLine($"Producer finished in {sw.ElapsedMilliseconds} ms");