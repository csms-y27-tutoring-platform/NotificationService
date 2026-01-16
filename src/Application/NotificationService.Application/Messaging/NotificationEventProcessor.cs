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

    public async Task ProcessBookingCreatedAsync(string bookingId, string createdBy, CancellationToken cancellationToken)
    {
        var notification = Notification.Create(
            Guid.NewGuid(),
            createdBy,
            "Booking Confirmed",
            $"Your booking {bookingId} has been successfully created. The tutor will contact you shortly.",
            NotificationType.BookingCreated);

        await _notificationRepository.AddAsync(notification, cancellationToken).ConfigureAwait(false);
        await _countRepository.IncrementCountAsync(createdBy, cancellationToken).ConfigureAwait(false);
    }

    public async Task ProcessBookingCancelledAsync(string bookingId, string cancelledBy, string reason, CancellationToken cancellationToken)
    {
        var notification = Notification.Create(
            Guid.NewGuid(),
            cancelledBy,
            "Booking Cancelled",
            $"Your booking {bookingId} has been cancelled. Reason: {reason}",
            NotificationType.BookingCancelled);

        await _notificationRepository.AddAsync(notification, cancellationToken).ConfigureAwait(false);
        await _countRepository.IncrementCountAsync(cancelledBy, cancellationToken).ConfigureAwait(false);
    }
}