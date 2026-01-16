namespace NotificationService.Application.Contracts.Events;

public record BookingCancelledEvent
{
    public string BookingId { get; init; } = string.Empty;

    public string CancelledBy { get; init; } = string.Empty;

    public string Reason { get; init; } = string.Empty;
}