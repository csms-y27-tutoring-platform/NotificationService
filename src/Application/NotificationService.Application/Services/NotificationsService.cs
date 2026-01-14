using NotificationService.Application.Abstractions.Repositories;
using NotificationService.Application.Contracts.DTO;
using NotificationService.Application.Contracts.Services;
using NotificationService.Application.Domain.Entities;
using NotificationService.Application.Mappers;

namespace NotificationService.Application.Services;

public class NotificationsService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnreadNotificationCountRepository _countRepository;

    public NotificationsService(
        INotificationRepository notificationRepository,
        IUnreadNotificationCountRepository countRepository)
    {
        _notificationRepository = notificationRepository;
        _countRepository = countRepository;
    }

    public async Task<IReadOnlyCollection<NotificationDto>> GetUserNotificationsAsync(Guid userId, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Notification> notifications = await _notificationRepository.FindByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);
        return notifications.Select(NotificationMapper.MapToDto).ToArray();
    }

    public async Task<NotificationDto?> GetNotificationByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Notification? notification = await _notificationRepository.FindByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return notification is null ? null : NotificationMapper.MapToDto(notification);
    }

    public async Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken)
    {
        Notification? notification = await _notificationRepository.FindByIdAsync(notificationId, cancellationToken).ConfigureAwait(false);

        if (notification is null)
        {
            throw new InvalidOperationException($"Notification with id {notificationId} not found");
        }

        notification.MarkAsRead();
        await _notificationRepository.UpdateAsync(notification, cancellationToken).ConfigureAwait(false);
        await _countRepository.DecrementCountAsync(notification.UserId, cancellationToken).ConfigureAwait(false);
    }

    public async Task<UnreadNotificationCountDto> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken)
    {
        int? count = await _countRepository.GetCountAsync(userId, cancellationToken).ConfigureAwait(false);

        if (count is null)
        {
            int actualCount = await _notificationRepository.CountUnreadByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);
            await _countRepository.UpdateCountAsync(userId, actualCount, cancellationToken).ConfigureAwait(false);
            count = actualCount;
        }

        return new UnreadNotificationCountDto
        {
            UserId = userId,
            Count = count.Value,
        };
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Notification> unreadNotifications = await _notificationRepository.FindUnreadByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);

        foreach (Notification notification in unreadNotifications)
        {
            notification.MarkAsRead();
            await _notificationRepository.UpdateAsync(notification, cancellationToken).ConfigureAwait(false);
        }

        await _countRepository.ResetCountAsync(userId, cancellationToken).ConfigureAwait(false);
    }
}