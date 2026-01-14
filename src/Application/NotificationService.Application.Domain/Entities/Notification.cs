using NotificationService.Application.Domain.Enums;

namespace NotificationService.Application.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public NotificationType Type { get; set; }

    public NotificationStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public Notification()
    {
    }

    public static Notification Create(Guid id, Guid userId, string title, string content, NotificationType type)
    {
        if (userId == Guid.Empty)
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