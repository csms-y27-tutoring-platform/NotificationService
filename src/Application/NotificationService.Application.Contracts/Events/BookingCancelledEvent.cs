namespace NotificationService.Application.Contracts.Events;

public record BookingCancelledEvent
{
    public long BookingId { get; init; }

    public string CancelledBy { get; init; } = string.Empty;

    public string Reason { get; init; } = string.Empty;
}