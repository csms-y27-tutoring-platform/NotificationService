using NotificationService.Application.Abstractions.Repositories;
using NotificationService.Infrastructure.Persistence.Connections;
using Npgsql;

namespace NotificationService.Infrastructure.Persistence.Repositories;

public class UnreadNotificationCountRepository : IUnreadNotificationCountRepository
{
    private readonly PostgresProvider _postgresProvider;

    public UnreadNotificationCountRepository(PostgresProvider postgresProvider)
    {
        _postgresProvider = postgresProvider;
    }

    public async Task<int?> GetCountAsync(string userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT unread_count
                           FROM unread_notification_counts
                           WHERE user_id = :user_id;
                           """;

        NpgsqlConnection connection = await _postgresProvider.OpenConnection().ConfigureAwait(false);
        await using (connection)
        {
            var command = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("user_id", userId),
                },
            };

            object? result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

            return result is not null ? Convert.ToInt32(result) : null;
        }
    }

    public async Task UpdateCountAsync(string userId, int count, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO unread_notification_counts (user_id, unread_count, updated_at)
                           VALUES (:user_id, :count, :updated_at)
                           ON CONFLICT (user_id) DO UPDATE SET
                               unread_count = EXCLUDED.unread_count,
                               updated_at = EXCLUDED.updated_at;
                           """;

        NpgsqlConnection connection = await _postgresProvider.OpenConnection().ConfigureAwait(false);
        await using (connection)
        {
            var command = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("user_id", userId),
                    new NpgsqlParameter("count", count),
                    new NpgsqlParameter("updated_at", DateTime.UtcNow),
                },
            };

            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task IncrementCountAsync(string userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO unread_notification_counts (user_id, unread_count, updated_at)
                           VALUES (:user_id, 1, :updated_at)
                           ON CONFLICT (user_id) DO UPDATE SET
                               unread_count = unread_notification_counts.unread_count + 1,
                               updated_at = EXCLUDED.updated_at;
                           """;

        NpgsqlConnection connection = await _postgresProvider.OpenConnection().ConfigureAwait(false);
        await using (connection)
        {
            var command = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("user_id", userId),
                    new NpgsqlParameter("updated_at", DateTime.UtcNow),
                },
            };

            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task DecrementCountAsync(string userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           UPDATE unread_notification_counts
                           SET unread_count = GREATEST(0, unread_count - 1),
                               updated_at = :updated_at
                           WHERE user_id = :user_id;
                           """;

        NpgsqlConnection connection = await _postgresProvider.OpenConnection().ConfigureAwait(false);
        await using (connection)
        {
            var command = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("user_id", userId),
                    new NpgsqlParameter("updated_at", DateTime.UtcNow),
                },
            };

            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task ResetCountAsync(string userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           UPDATE unread_notification_counts
                           SET unread_count = 0,
                               updated_at = :updated_at
                           WHERE user_id = :user_id;
                           """;

        NpgsqlConnection connection = await _postgresProvider.OpenConnection().ConfigureAwait(false);
        await using (connection)
        {
            var command = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("user_id", userId),
                    new NpgsqlParameter("updated_at", DateTime.UtcNow),
                },
            };

            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}