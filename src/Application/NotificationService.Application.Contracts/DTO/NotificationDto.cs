using NotificationService.Application.Domain.Enums;

namespace NotificationService.Application.Contracts.DTO;

public record NotificationDto
{
    public Guid Id { get; init; }

    public string UserId { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public NotificationType Type { get; init; }

    public NotificationStatus Status { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? ReadAt { get; init; }
}