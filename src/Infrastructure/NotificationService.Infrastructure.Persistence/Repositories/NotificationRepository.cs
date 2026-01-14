using NotificationService.Application.Abstractions.Repositories;
using NotificationService.Application.Domain.Entities;
using NotificationService.Application.Domain.Enums;
using NotificationService.Infrastructure.Persistence.Connections;
using Npgsql;

namespace NotificationService.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly PostgresProvider _postgresProvider;

    public NotificationRepository(PostgresProvider postgresProvider)
    {
        _postgresProvider = postgresProvider;
    }

    public async Task<Notification?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT id, user_id, title, content, type, status, created_at, read_at
                           FROM notifications
                           WHERE id = :id;
                           """;

        NpgsqlConnection connection = await _postgresProvider.OpenConnection().ConfigureAwait(false);
        await using (connection)
        {
            var command = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("id", id),
                },
            };

            NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            await using (reader)
            {
                if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    return MapNotification(reader);
                }

                return null;
            }
        }
    }

    public async Task<IReadOnlyCollection<Notification>> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT id, user_id, title, content, type, status, created_at, read_at
                           FROM notifications
                           WHERE user_id = :user_id
                           ORDER BY created_at DESC;
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

            NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            await using (reader)
            {
                var notifications = new List<Notification>();

                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    notifications.Add(MapNotification(reader));
                }

                return notifications.AsReadOnly();
            }
        }
    }

    public async Task<IReadOnlyCollection<Notification>> FindUnreadByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT id, user_id, title, content, type, status, created_at, read_at
                           FROM notifications
                           WHERE user_id = :user_id AND status = :status
                           ORDER BY created_at DESC;
                           """;

        NpgsqlConnection connection = await _postgresProvider.OpenConnection().ConfigureAwait(false);
        await using (connection)
        {
            var command = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("user_id", userId),
                    new NpgsqlParameter("status", NotificationStatus.Unread),
                },
            };

            NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            await using (reader)
            {
                var notifications = new List<Notification>();

                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    notifications.Add(MapNotification(reader));
                }

                return notifications.AsReadOnly();
            }
        }
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO notifications (id, user_id, title, content, type, status, created_at, read_at)
                           VALUES (:id, :user_id, :title, :content, :type, :status, :created_at, :read_at);
                           """;

        NpgsqlConnection connection = await _postgresProvider.OpenConnection().ConfigureAwait(false);
        await using (connection)
        {
            var command = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("id", notification.Id),
                    new NpgsqlParameter("user_id", notification.UserId),
                    new NpgsqlParameter("title", notification.Title),
                    new NpgsqlParameter("content", notification.Content),
                    new NpgsqlParameter("type", notification.Type),
                    new NpgsqlParameter("status", notification.Status),
                    new NpgsqlParameter("created_at", notification.CreatedAt),
                    new NpgsqlParameter("read_at", notification.ReadAt ?? (object)DBNull.Value),
                },
            };

            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken)
    {
        const string sql = """
                           UPDATE notifications
                           SET user_id = :user_id,
                               title = :title,
                               content = :content,
                               type = :type,
                               status = :status,
                               created_at = :created_at,
                               read_at = :read_at
                           WHERE id = :id;
                           """;

        NpgsqlConnection connection = await _postgresProvider.OpenConnection().ConfigureAwait(false);
        await using (connection)
        {
            var command = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("id", notification.Id),
                    new NpgsqlParameter("user_id", notification.UserId),
                    new NpgsqlParameter("title", notification.Title),
                    new NpgsqlParameter("content", notification.Content),
                    new NpgsqlParameter("type", notification.Type),
                    new NpgsqlParameter("status", notification.Status),
                    new NpgsqlParameter("created_at", notification.CreatedAt),
                    new NpgsqlParameter("read_at", notification.ReadAt ?? (object)DBNull.Value),
                },
            };

            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task<int> CountUnreadByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT COUNT(*)
                           FROM notifications
                           WHERE user_id = :user_id AND status = :status;
                           """;

        NpgsqlConnection connection = await _postgresProvider.OpenConnection().ConfigureAwait(false);
        await using (connection)
        {
            var command = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("user_id", userId),
                    new NpgsqlParameter("status", NotificationStatus.Unread),
                },
            };

            object? result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

            return result is not null ? Convert.ToInt32(result) : 0;
        }
    }

    private static Notification MapNotification(NpgsqlDataReader reader)
    {
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        Guid userId = reader.GetGuid(reader.GetOrdinal("user_id"));
        string title = reader.GetString(reader.GetOrdinal("title"));
        string content = reader.GetString(reader.GetOrdinal("content"));
        NotificationType type = reader.GetFieldValue<NotificationType>(reader.GetOrdinal("type"));
        NotificationStatus status = reader.GetFieldValue<NotificationStatus>(reader.GetOrdinal("status"));
        DateTime createdAt = reader.GetDateTime(reader.GetOrdinal("created_at"));
        DateTime? readAt = reader.IsDBNull(reader.GetOrdinal("read_at")) ? null : reader.GetDateTime(reader.GetOrdinal("read_at"));

        var notification = Notification.Create(
            id,
            userId,
            title,
            content,
            type);

        if (status == NotificationStatus.Read && readAt.HasValue)
        {
            notification.MarkAsRead();
        }

        return notification;
    }
}