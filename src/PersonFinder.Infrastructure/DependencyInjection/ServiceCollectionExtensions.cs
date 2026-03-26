using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonFinder.Application.Abstractions;
using PersonFinder.Domain.Repositories;
using PersonFinder.Infrastructure.BackgroundServices;
using PersonFinder.Infrastructure.ExternalServices;
using PersonFinder.Infrastructure.Persistence;
using PersonFinder.Infrastructure.Persistence.Repositories;

namespace PersonFinder.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PersonFinderDb");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'PersonFinderDb' is required.");
        }

        services.AddDbContext<PersonFinderDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // Gemini AI service (typed HTTP client)
        services.AddHttpClient<IGeminiService, GeminiService>();

        // Bio generation queue (singleton channel shared across all request scopes)
        services.AddSingleton<BioQueue>();
        services.AddSingleton<IBioQueue>(sp => sp.GetRequiredService<BioQueue>());

        // Background worker that consumes the queue and calls Gemini
        services.AddHostedService<BioGenerationBackgroundService>();

        return services;
    }
}
