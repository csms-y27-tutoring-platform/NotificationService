using NotificationService.Application.Abstractions.Messaging;
using NotificationService.Application.Abstractions.Repositories;
using NotificationService.Application.Domain.Entities;
using NotificationService.Application.Domain.Enums;

namespace NotificationService.Application.Messaging;

public class NotificationEventProcessor : INotificationEventProcessor
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnreadNotificationCountRepository _countRepository;

    public NotificationEventProcessor(
        INotificationRepository notificationRepository,
        IUnreadNotificationCountRepository countRepository)
    {
        _notificationRepository = notificationRepository;
        _countRepository = countRepository;
    }

    public async Task ProcessBookingCreatedAsync(long bookingId, string createdBy, CancellationToken cancellationToken)
    {
        Guid userId = ParseUserId(createdBy);
        var notification = Notification.Create(
            Guid.NewGuid(),
            userId,
            "Booking Confirmed",
            $"Your booking #{bookingId} has been successfully created. The tutor will contact you shortly.",
            NotificationType.BookingCreated);

        await _notificationRepository.AddAsync(notification, cancellationToken).ConfigureAwait(false);
        await _countRepository.IncrementCountAsync(userId, cancellationToken).ConfigureAwait(false);
    }

    public async Task ProcessBookingCancelledAsync(long bookingId, string cancelledBy, string reason, CancellationToken cancellationToken)
    {
        Guid userId = ParseUserId(cancelledBy);
        var notification = Notification.Create(
            Guid.NewGuid(),
            userId,
            "Booking Cancelled",
            $"Your booking #{bookingId} has been cancelled. Reason: {reason}",
            NotificationType.BookingCancelled);

        await _notificationRepository.AddAsync(notification, cancellationToken).ConfigureAwait(false);
        await _countRepository.IncrementCountAsync(userId, cancellationToken).ConfigureAwait(false);
    }

    private static Guid ParseUserId(string userString)
    {
        if (Guid.TryParse(userString, out Guid userId))
        {
            return userId;
        }

        throw new ArgumentException($"Invalid user ID format: {userString}", nameof(userString));
    }
}