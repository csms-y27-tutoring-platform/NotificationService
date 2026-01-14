namespace NotificationService.Application.Contracts.Events;

public record BookingCreatedEvent
{
    public long BookingId { get; init; }

    public string CreatedBy { get; init; } = string.Empty;
}