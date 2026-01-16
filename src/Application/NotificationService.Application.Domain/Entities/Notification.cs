using NotificationService.Application.Domain.Enums;

namespace NotificationService.Application.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }

    public string UserId { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public string Content { get; private set; } = string.Empty;

    public NotificationType Type { get; private set; }

    public NotificationStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ReadAt { get; private set; }

    private Notification()
    {
    }

    public static Notification Create(Guid id, string userId, string title, string content, NotificationType type)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        }

        var notification = new Notification
        {
            Id = id,
            UserId = userId,
            Title = title,
            Content = content,
            Type = type,
            Status = NotificationStatus.Unread,
            CreatedAt = DateTime.UtcNow,
        };

        return notification;
    }

    public void MarkAsRead()
    {
        if (Status == NotificationStatus.Read)
        {
            return;
        }

        Status = NotificationStatus.Read;
        ReadAt = DateTime.UtcNow;
    }
}