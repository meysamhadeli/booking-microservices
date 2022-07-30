namespace BuildingBlocks.PersistMessageProcessor;

[Flags]
public enum MessageDeliveryType
{
    Outbox = 1,
    Inbox = 2,
    Internal = 4
}
