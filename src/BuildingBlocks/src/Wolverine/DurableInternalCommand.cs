using System.Text.Json;
using Woo.Core.Event;
using Woo.Utils;
using MediatR;

namespace Woo.Wolverine;

public sealed record DurableInternalCommand(string DataType, string Data);

public class DurableInternalCommandHandler
{
    private readonly IMediator _mediator;

    public DurableInternalCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(DurableInternalCommand message, CancellationToken cancellationToken)
    {
        var commandType = TypeProvider.GetFirstMatchingTypeFromCurrentDomainAssembly(message.DataType);

        if (commandType is null)
        {
            throw new InvalidOperationException($"Could not resolve internal command type '{message.DataType}'.");
        }

        var command = JsonSerializer.Deserialize(message.Data, commandType);

        if (command is not IInternalCommand internalCommand)
        {
            throw new InvalidOperationException($"Deserialized message '{message.DataType}' is not an internal command.");
        }

        await _mediator.Send(internalCommand, cancellationToken);
    }
}