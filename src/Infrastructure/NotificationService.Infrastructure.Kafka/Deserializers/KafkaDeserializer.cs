using Confluent.Kafka;
using Google.Protobuf;

namespace NotificationService.Infrastructure.Kafka.Deserializers;

public class KafkaDeserializer<T> : IDeserializer<T> where T : IMessage<T>, new()
{
    private readonly MessageParser<T> _parser = new MessageParser<T>(() => new T());

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
        {
            throw new ArgumentNullException(nameof(data));
        }

        return _parser.ParseFrom(data);
    }
}