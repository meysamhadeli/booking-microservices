using BuildingBlocks.Core.CQRS;
using BuildingBlocks.IdsGenerator;

namespace BuildingBlocks.Core.Event;

public record InternalCommand : IInternalCommand, ICommand;
