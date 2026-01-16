using NotificationService.Application.Contracts.DTO;

namespace NotificationService.Application.Contracts.Services;

public interface INotificationService
{
    public Task<IReadOnlyCollection<NotificationDto>> GetUserNotificationsAsync(string userId, CancellationToken cancellationToken);

    public Task<NotificationDto?> GetNotificationByIdAsync(string id, CancellationToken cancellationToken);

    public Task MarkAsReadAsync(string notificationId, CancellationToken cancellationToken);

    public Task<UnreadNotificationCountDto> GetUnreadCountAsync(string userId, CancellationToken cancellationToken);

    public Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken);
}