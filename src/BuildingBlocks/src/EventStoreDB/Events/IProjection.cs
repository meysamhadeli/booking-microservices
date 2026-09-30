namespace Woo.EventStoreDB.Events;

public interface IProjection
{
    void When(object @event);
}