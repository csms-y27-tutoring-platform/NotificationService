using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NotificationService.Application.Abstractions.Repositories;
using NotificationService.Infrastructure.Persistence.Connections;
using NotificationService.Infrastructure.Persistence.Migrations;
using NotificationService.Infrastructure.Persistence.Repositories;
using Npgsql;

namespace NotificationService.Infrastructure.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PostgresOptions>().Bind(configuration.GetSection("Infrastructure:Persistence:Postgres"));
        services.AddSingleton<NpgsqlDataSourceBuilder>(_ => new NpgsqlDataSourceBuilder());
        services.AddSingleton<PostgresProvider>();

        services.AddFluentMigratorCore()
            .ConfigureRunner(runner =>
                runner.AddPostgres()
                    .WithGlobalConnectionString(provider =>
                    {
                        NpgsqlDataSourceBuilder postgresBuilder =
                            provider.GetRequiredService<NpgsqlDataSourceBuilder>();
                        PostgresOptions postgresOptions =
                            provider.GetRequiredService<IOptions<PostgresOptions>>().Value;
                        NpgsqlConnectionStringBuilder connectionString = postgresBuilder.ConnectionStringBuilder;
                        connectionString.Database = postgresOptions.Database;
                        connectionString.Host = postgresOptions.Host;
                        connectionString.Port = postgresOptions.Port;
                        connectionString.Username = postgresOptions.Username;
                        connectionString.Password = postgresOptions.Password;
                        return connectionString.ToString();
                    })
                    .WithMigrationsIn(typeof(InitialMigration).Assembly));

        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IUnreadNotificationCountRepository, UnreadNotificationCountRepository>();

        return services;
    }
}