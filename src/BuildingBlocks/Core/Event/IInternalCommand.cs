using BuildingBlocks.Core.CQRS;

namespace BuildingBlocks.Core.Event;

public interface IInternalCommand : ICommand
{
    long Id { get; }
    DateTime OccurredOn { get; }
    string Type { get; }
}
