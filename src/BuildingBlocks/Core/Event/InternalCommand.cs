using BuildingBlocks.Core.CQRS;

namespace BuildingBlocks.Core.Event;

public record InternalCommand : IInternalCommand, ICommand;
