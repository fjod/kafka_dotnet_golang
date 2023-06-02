// See https://aka.ms/new-console-template for more information

using Common;
using Common.StudyTypes;
using Consumer;

Console.WriteLine("Consumer starting..");

var configuration = Settings.Get(false);
ClusterCheck clusterCheck = new ClusterCheck();
if (!clusterCheck.CheckIfClusterIsAvailable(configuration)) return;

var studyType = Settings.GetStudyType<ConsumerStudyType>();
Console.WriteLine(studyType != null ? $"Found env var studyType =  {studyType}" : "No env var studyType found, using SimpleConsume");

var study = TestingFactory.Get(studyType ?? ConsumerStudyType.SimpleConsume);
await study(configuration);