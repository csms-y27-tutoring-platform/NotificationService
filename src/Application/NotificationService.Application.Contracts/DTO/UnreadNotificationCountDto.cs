namespace NotificationService.Application.Contracts.DTO;

public record UnreadNotificationCountDto
{
    public string UserId { get; init; } = string.Empty;

    public int Count { get; init; }
}