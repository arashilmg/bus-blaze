using Microsoft.Extensions.Configuration;

namespace BizCover.Application.Renewals.Configuration;

public static class RenewalConfigHelper
{
    internal static T GetConfig<T>(string fileName)
    {
        var configurationRoot = BuildConfig(fileName);
        return LoadConfig<T>(configurationRoot);
    }

    private static T LoadConfig<T>(IConfiguration configuration)
    {
        var section = configuration.GetSection(typeof(T).Name);
        if (section == null)
        {
            throw new System.Exception($"Configuration section named '{typeof(T).Name}' could not be found");
        }

        var config = section.Get<T>();

        if (config == null)
        {
            throw new System.Exception($"Configuration section named '{typeof(T).Name}' is not valid");
        }

        var test = (T)Convert.ChangeType(config, typeof(T));
        return test;
    }

    private static IConfigurationRoot BuildConfig(string fileName)
    {
        var configurationRoot = new ConfigurationBuilder()
            .AddJsonFile(fileName)
            .Build();
        return configurationRoot;
    }
}