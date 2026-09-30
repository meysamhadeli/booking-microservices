using Woo.Core.CQRS;

namespace Woo.Core.Event;

public record InternalCommand : IInternalCommand, ICommand;