using System.Runtime.CompilerServices;
using KnabEngineerRecruitmentTestInfrastructure.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly:InternalsVisibleTo("KnabEngineerRecruitmentTestTests")]
namespace KnabEngineerRecruitmentTestInfrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddKnabEngineerRecruitmentTestInfrastructure(this IServiceCollection services,
        IConfigurationRoot configuration) =>
        services.AddSingleton<IQuotesApiClient, QuotesApiClient>()
            .Configure<ExternalQuotesApiSettings>(
                configuration.GetSection(nameof(ExternalQuotesApiSettings)));
}