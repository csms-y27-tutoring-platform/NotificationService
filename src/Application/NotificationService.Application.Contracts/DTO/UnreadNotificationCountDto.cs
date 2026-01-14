namespace NotificationService.Application.Contracts.DTO;

public record UnreadNotificationCountDto
{
    public Guid UserId { get; init; }

    public int Count { get; init; }
}