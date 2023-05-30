using Microsoft.Extensions.Configuration;
using Producer.StudyTypes;

namespace Producer;

public static class Settings
{
    public static IConfiguration Get()
    {
        var exePath = System.Reflection.Assembly.GetEntryAssembly()?.Location;
        if (exePath == null) throw new Exception("Cant find current location");
        var basePath = Path.GetDirectoryName(exePath);
        if (basePath == null) throw new Exception("Cant create base path for settings");
        var propPath = Path.Combine(basePath, "producer.properties");
        return new ConfigurationBuilder()
            .AddIniFile(propPath)
            .Build();
    }

    public static StudyType? GetStudyType()
    {
        var value = Environment.GetEnvironmentVariable("STUDY_TYPE");
        if (value == null) return null;

        if (Enum.TryParse(value, out StudyType type))
        {
            return type;
        }

        return null;
    }
}