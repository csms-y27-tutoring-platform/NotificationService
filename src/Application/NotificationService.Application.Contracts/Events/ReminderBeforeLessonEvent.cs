namespace NotificationService.Application.Contracts.Events;

public record ReminderBeforeLessonEvent
{
    public long BookingId { get; init; }

    public DateTime LessonTime { get; init; }

    public Guid StudentId { get; init; }

    public Guid TutorId { get; init; }

    public string Subject { get; init; } = string.Empty;
}