using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BizCover.Application.Renewals.UseCases.WordingChanges;

public static class ServiceCollectionExtension
{
    private static IConfiguration Configuration =>
        new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile(Path.Join("UseCases","WordingChanges","WordingChangesConfig.json"))
            .Build();

    public static void AddWordingChangesConfig(this IServiceCollection services) => 
        services.AddSingleton(Configuration.GetSection(nameof(WordingChangesConfig)).Get<WordingChangesConfig>());
}
