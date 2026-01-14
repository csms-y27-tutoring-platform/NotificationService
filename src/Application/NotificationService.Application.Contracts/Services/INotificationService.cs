using NotificationService.Application.Contracts.DTO;

namespace NotificationService.Application.Contracts.Services;

public interface INotificationService
{
    public Task<IReadOnlyCollection<NotificationDto>> GetUserNotificationsAsync(Guid userId, CancellationToken cancellationToken);

    public Task<NotificationDto?> GetNotificationByIdAsync(Guid id, CancellationToken cancellationToken);

    public Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken);

    public Task<UnreadNotificationCountDto> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken);

    public Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken);
}