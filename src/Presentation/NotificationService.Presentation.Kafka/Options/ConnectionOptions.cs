using Confluent.Kafka;

namespace NotificationService.Presentation.Kafka.Options;

public class ConnectionOptions
{
    public string Host { get; set; } = string.Empty;

    public SecurityProtocol SecurityProtocol { get; set; }
}