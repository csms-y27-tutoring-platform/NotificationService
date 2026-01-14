using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Contracts.Services;

namespace NotificationService.Presentation.Grpc.Services;

public class NotificationGrpcService : NotificationService.NotificationServiceBase
{
    private readonly INotificationService _notificationService;

    public NotificationGrpcService(
        INotificationService notificationService,
        ILogger<NotificationGrpcService> logger)
    {
        _notificationService = notificationService;
    }

    public new async Task<GetNotificationsResponse> GetNotifications(
        GetNotificationsRequest request,
        ServerCallContext context)
    {
        var userId = new Guid(request.UserId);

        IReadOnlyCollection<Application.Contracts.DTO.NotificationDto> notifications =
            await _notificationService.GetUserNotificationsAsync(userId, context.CancellationToken).ConfigureAwait(false);

        var response = new GetNotificationsResponse();
        response.Notifications.AddRange(notifications.Select(Mappers.NotificationMapper.MapToGrpc));

        return response;
    }

    public override async Task<Notification> GetNotification(
        GetNotificationRequest request,
        ServerCallContext context)
    {
        var notificationId = new Guid(request.NotificationId);

        Application.Contracts.DTO.NotificationDto? notification =
            await _notificationService.GetNotificationByIdAsync(notificationId, context.CancellationToken).ConfigureAwait(false);

        if (notification is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Notification with id {notificationId} not found"));
        }

        return Mappers.NotificationMapper.MapToGrpc(notification);
    }

    public override async Task<GetUnreadCountResponse> GetUnreadCount(
        GetUnreadCountRequest request,
        ServerCallContext context)
    {
        var userId = new Guid(request.UserId);

        Application.Contracts.DTO.UnreadNotificationCountDto countDto =
            await _notificationService.GetUnreadCountAsync(userId, context.CancellationToken).ConfigureAwait(false);

        return new GetUnreadCountResponse
        {
            Count = countDto.Count,
        };
    }

    public override async Task<Empty> MarkAsRead(
        MarkAsReadRequest request,
        ServerCallContext context)
    {
        var notificationId = new Guid(request.NotificationId);

        await _notificationService.MarkAsReadAsync(notificationId, context.CancellationToken).ConfigureAwait(false);

        return new Empty();
    }

    public override async Task<Empty> MarkAllAsRead(
        MarkAllAsReadRequest request,
        ServerCallContext context)
    {
        var userId = new Guid(request.UserId);

        await _notificationService.MarkAllAsReadAsync(userId, context.CancellationToken).ConfigureAwait(false);

        return new Empty();
    }
}