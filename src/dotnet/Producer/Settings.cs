using Microsoft.Extensions.Configuration;

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
}