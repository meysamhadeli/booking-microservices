namespace BuildingBlocks.PersistMessageProcessor;

[Flags]
public enum MessageDeliveryType
{
    Unknown = 0,
    Outbox = 1,
    Inbox = 2,
    Internal = 3
}