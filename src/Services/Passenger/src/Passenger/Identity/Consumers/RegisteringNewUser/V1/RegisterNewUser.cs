namespace Passenger.Identity.Consumers.RegisteringNewUser.V1;

using Ardalis.GuardClauses;
using Woo.Core.EventBus.Messages;
using Woo.Core;
using Woo.Core.Event;
using Woo.Web;
using Data;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Passengers.ValueObjects;

public class RegisterNewUserHandler
{
    private readonly PassengerDbContext _passengerDbContext;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ILogger<RegisterNewUserHandler> _logger;
    private readonly AppOptions _options;

    public RegisterNewUserHandler(PassengerDbContext passengerDbContext,
        IEventDispatcher eventDispatcher,
        ILogger<RegisterNewUserHandler> logger,
        IOptions<AppOptions> options)
    {
        _passengerDbContext = passengerDbContext;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
        _options = options.Value;
    }

    public async Task Handle(UserCreated message, CancellationToken cancellationToken)
    {
        Guard.Against.Null(message, nameof(message));

        _logger.LogInformation($"consumer for {nameof(UserCreated).Underscore()} in {_options.Name}");

        var passengerExist =
            await _passengerDbContext.Passengers.AnyAsync(
                x => x.PassportNumber.Value == message.PassportNumber,
                cancellationToken);

        if (passengerExist)
        {
            return;
        }

        var passenger = Passengers.Models.Passenger.Create(
            PassengerId.Of(message.Id),
            Name.Of(message.Name),
            PassportNumber.Of(message.PassportNumber));

        await _passengerDbContext.AddAsync(passenger, cancellationToken);

        await _passengerDbContext.SaveChangesAsync(cancellationToken);

        await _eventDispatcher.SendAsync(
            new PassengerCreatedDomainEvent(passenger.Id, passenger.Name, passenger.PassportNumber),
            typeof(IInternalCommand),
            cancellationToken);
    }
}