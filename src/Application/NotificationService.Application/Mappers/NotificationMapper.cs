using NotificationService.Application.Contracts.DTO;
using NotificationService.Application.Domain.Entities;

namespace NotificationService.Application.Mappers;

public static class NotificationMapper
{
    public static NotificationDto MapToDto(Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Title = notification.Title,
            Content = notification.Content,
            Type = notification.Type,
            Status = notification.Status,
            CreatedAt = notification.CreatedAt,
            ReadAt = notification.ReadAt,
        };
    }
}