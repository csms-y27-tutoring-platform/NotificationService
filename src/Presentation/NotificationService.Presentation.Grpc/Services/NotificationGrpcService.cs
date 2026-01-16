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

    public override async Task<GetNotificationsResponse> GetNotifications(
        GetNotificationsRequest request,
        ServerCallContext context)
    {
        IReadOnlyCollection<Application.Contracts.DTO.NotificationDto> notifications =
            await _notificationService.GetUserNotificationsAsync(request.UserId, context.CancellationToken).ConfigureAwait(false);

        var response = new GetNotificationsResponse();
        response.Notifications.AddRange(notifications.Select(Mappers.NotificationMapper.MapToGrpc));

        return response;
    }

    public override async Task<Notification> GetNotification(
        GetNotificationRequest request,
        ServerCallContext context)
    {
        string notificationId = request.NotificationId;

        Application.Contracts.DTO.NotificationDto? notification =
            await _notificationService.GetNotificationByIdAsync(notificationId, context.CancellationToken).ConfigureAwait(false);

        if (notification is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Notification with id {request.NotificationId} not found"));
        }

        return Mappers.NotificationMapper.MapToGrpc(notification);
    }

    public override async Task<GetUnreadCountResponse> GetUnreadCount(
        GetUnreadCountRequest request,
        ServerCallContext context)
    {
        Application.Contracts.DTO.UnreadNotificationCountDto countDto =
            await _notificationService.GetUnreadCountAsync(request.UserId, context.CancellationToken).ConfigureAwait(false);

        return new GetUnreadCountResponse
        {
            Count = countDto.Count,
        };
    }

    public override async Task<Empty> MarkAsRead(
        MarkAsReadRequest request,
        ServerCallContext context)
    {
        string notificationId = request.NotificationId;

        await _notificationService.MarkAsReadAsync(notificationId, context.CancellationToken).ConfigureAwait(false);

        return new Empty();
    }

    public override async Task<Empty> MarkAllAsRead(
        MarkAllAsReadRequest request,
        ServerCallContext context)
    {
        await _notificationService.MarkAllAsReadAsync(request.UserId, context.CancellationToken).ConfigureAwait(false);

        return new Empty();
    }
}