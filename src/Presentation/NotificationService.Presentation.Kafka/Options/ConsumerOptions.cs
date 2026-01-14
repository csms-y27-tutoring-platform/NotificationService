namespace NotificationService.Presentation.Kafka.Options;

public class ConsumerOptions
{
    public string GroupId { get; set; } = string.Empty;

    public string BookingCreatedTopic { get; set; } = string.Empty;

    public string BookingCancelledTopic { get; set; } = string.Empty;

    public string BookingCompletedTopic { get; set; } = string.Empty;

    public string ReminderBeforeLessonTopic { get; set; } = string.Empty;
}