using Google.Protobuf;

namespace BuildingBlocks.Core.Event;

public class MessageEnvelope
{
    public MessageEnvelope(object? message, IDictionary<string, object?>? headers = null)
    {
        Message = message;
        Headers = headers ?? new Dictionary<string, object?>();
    }

    public object? Message { get; init; }
    public IDictionary<string, object?> Headers { get; init; }
}

public class MessageEnvelope<TMessage> : MessageEnvelope
    where TMessage : class, IMessage
{
    public MessageEnvelope(TMessage message, IDictionary<string, object?> header) : base(message, header)
    {
        Message = message;
    }

    public new TMessage? Message { get; }
}
