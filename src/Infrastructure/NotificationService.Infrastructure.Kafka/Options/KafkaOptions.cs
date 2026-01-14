using Confluent.Kafka;

namespace NotificationService.Infrastructure.Kafka.Options;

public class KafkaOptions
{
    public string BootstrapServers { get; set; } = string.Empty;

    public SecurityProtocol SecurityProtocol { get; set; }

    public string GroupId { get; set; } = string.Empty;

    public string BookingCreatedTopic { get; set; } = string.Empty;

    public string BookingCancelledTopic { get; set; } = string.Empty;

    public string BookingCompletedTopic { get; set; } = string.Empty;

    public string ReminderBeforeLessonTopic { get; set; } = string.Empty;

    public int MaxPollIntervalMs { get; set; } = 300000;

    public int SessionTimeoutMs { get; set; } = 10000;

    public bool EnableAutoCommit { get; set; } = false;

    public int AutoCommitIntervalMs { get; set; } = 5000;
}