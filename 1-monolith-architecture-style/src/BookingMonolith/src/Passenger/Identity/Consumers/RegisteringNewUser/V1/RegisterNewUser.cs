using Ardalis.GuardClauses;
using BookingMonolith.Passenger.Data;
using BookingMonolith.Passenger.Passengers.ValueObjects;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Core;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Web;
using Humanizer;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookingMonolith.Passenger.Identity.Consumers.RegisteringNewUser.V1;

public class RegisterNewUserHandler : IConsumer<UserCreated>
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

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        Guard.Against.Null(context.Message, nameof(UserCreated));

        _logger.LogInformation($"consumer for {nameof(UserCreated).Underscore()} in {_options.Name}");

        var passengerExist =
            await _passengerDbContext.Passengers.AnyAsync(x => x.PassportNumber.Value == context.Message.PassportNumber);

        if (passengerExist)
        {
            return;
        }

        var passenger = Passengers.Models.Passenger.Create(PassengerId.Of(NewId.NextGuid()), Name.Of(context.Message.Name),
            PassportNumber.Of(context.Message.PassportNumber));

        await _passengerDbContext.AddAsync(passenger);

        await _passengerDbContext.SaveChangesAsync();

        await _eventDispatcher.SendAsync(
            new PassengerCreatedDomainEvent(passenger.Id, passenger.Name, passenger.PassportNumber),
            typeof(IInternalCommand));
    }
}
