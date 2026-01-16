using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;

namespace NotificationService.Infrastructure.Persistence.Migrations;

[Migration(version: 20240101000000, description: "Initial migration")]
public class InitialMigration : IMigration
{
    public void GetUpExpressions(IMigrationContext context)
    {
        context.Expressions.Add(new ExecuteSqlStatementExpression
        {
            SqlStatement = """
                           do $$
                           begin
                                create type notification_status as enum ('unread', 'read');
                           exception
                                when duplicate_object then null;
                           end$$;
                           
                           do $$
                           begin
                                create type notification_type as enum ('booking_created', 'booking_cancelled', 'reminder');
                           exception
                                when duplicate_object then null;
                           end$$;

                           create table if not exists notifications
                           (
                               id         uuid primary key,
                               user_id    text                     not null,
                               title      text                     not null,
                               content    text                     not null,
                               type       notification_type        not null,
                               status     notification_status      not null,
                               created_at timestamp with time zone not null,
                               read_at    timestamp with time zone null
                           );

                           create index if not exists notifications_user_id_idx on notifications (user_id);
                           
                           create table if not exists unread_notification_counts
                           (
                               user_id      text primary key,
                               unread_count integer                  not null,
                               updated_at   timestamp with time zone not null
                           );
                           """,
        });
    }

    public void GetDownExpressions(IMigrationContext context)
    {
        context.Expressions.Add(new ExecuteSqlStatementExpression
        {
            SqlStatement = """
                           drop table if exists unread_notification_counts;
                           drop table if exists notifications;
                           drop type if exists notification_type;
                           drop type if exists notification_status;
                           """,
        });
    }

    public string ConnectionString => throw new NotSupportedException();
}