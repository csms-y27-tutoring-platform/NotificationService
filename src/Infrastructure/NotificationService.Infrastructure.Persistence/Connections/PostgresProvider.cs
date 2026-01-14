using Microsoft.Extensions.Options;
using NotificationService.Application.Domain.Enums;
using Npgsql;

namespace NotificationService.Infrastructure.Persistence.Connections;

public class PostgresProvider
{
    private readonly NpgsqlDataSource _npgsqlDataSource;

    public PostgresProvider(NpgsqlDataSourceBuilder npgsqlDataSourceBuilder, IOptions<PostgresOptions> postgresOptions)
    {
        PostgresOptions options = postgresOptions.Value;
        NpgsqlConnectionStringBuilder connectionString = npgsqlDataSourceBuilder.ConnectionStringBuilder;
        connectionString.Host = options.Host;
        connectionString.Username = options.Username;
        connectionString.Password = options.Password;
        connectionString.Port = options.Port;
        connectionString.Database = options.Database;

        npgsqlDataSourceBuilder.MapEnum<NotificationStatus>(pgName: "notification_status");
        npgsqlDataSourceBuilder.MapEnum<NotificationType>(pgName: "notification_type");

        _npgsqlDataSource = npgsqlDataSourceBuilder.Build();
    }

    public async Task<NpgsqlConnection> OpenConnection()
    {
        return await _npgsqlDataSource.OpenConnectionAsync().ConfigureAwait(false);
    }
}