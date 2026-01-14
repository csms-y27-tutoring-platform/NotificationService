using NotificationService.Application.Contracts.DTO;

namespace NotificationService.Presentation.Grpc.Mappers;

public static class NotificationMapper
{
    public static Notification MapToGrpc(NotificationDto dto)
    {
        var notification = new Notification
        {
            Id = dto.Id.ToString(),
            UserId = dto.UserId.ToString(),
            Title = dto.Title,
            Content = dto.Content,
            Type = MapTypeToGrpc(dto.Type),
            Status = MapStatusToGrpc(dto.Status),
            CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(dto.CreatedAt),
        };

        if (dto.ReadAt.HasValue)
        {
            notification.ReadAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(dto.ReadAt.Value);
        }

        return notification;
    }

    public static NotificationType MapTypeToGrpc(Application.Domain.Enums.NotificationType type)
    {
        return type switch
        {
            Application.Domain.Enums.NotificationType.BookingCreated => NotificationType.BookingCreated,
            Application.Domain.Enums.NotificationType.BookingCancelled => NotificationType.BookingCancelled,
            Application.Domain.Enums.NotificationType.Reminder => NotificationType.Reminder,
            _ => NotificationType.BookingCreated,
        };
    }

    public static NotificationStatus MapStatusToGrpc(Application.Domain.Enums.NotificationStatus status)
    {
        return status switch
        {
            Application.Domain.Enums.NotificationStatus.Unread => NotificationStatus.Unread,
            Application.Domain.Enums.NotificationStatus.Read => NotificationStatus.Read,
            _ => NotificationStatus.Unread,
        };
    }
}