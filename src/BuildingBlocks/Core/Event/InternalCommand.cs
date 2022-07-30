using BuildingBlocks.Core.CQRS;
using BuildingBlocks.IdsGenerator;

namespace BuildingBlocks.Core.Event;

public class InternalCommand : IInternalCommand, ICommand
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}
