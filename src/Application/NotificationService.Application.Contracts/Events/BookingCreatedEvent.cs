namespace NotificationService.Application.Contracts.Events;

public record BookingCreatedEvent
{
    public string BookingId { get; init; } = string.Empty;

    public string CreatedBy { get; init; } = string.Empty;
}