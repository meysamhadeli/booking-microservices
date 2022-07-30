using Ardalis.GuardClauses;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Core;
using BuildingBlocks.Core.Event;
using BuildingBlocks.IdsGenerator;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Passenger.Data;
using Passenger.Passengers.Events.Domain;

namespace Passenger.Identity.RegisterNewUser;

public class RegisterNewUserConsumerHandler : IConsumer<UserCreated>
{
    private readonly PassengerDbContext _passengerDbContext;
    private readonly IEventDispatcher _eventDispatcher;

    public RegisterNewUserConsumerHandler(PassengerDbContext passengerDbContext,
        IEventDispatcher eventDispatcher)
    {
        _passengerDbContext = passengerDbContext;
        _eventDispatcher = eventDispatcher;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        Guard.Against.Null(context.Message, nameof(UserCreated));

        var passengerExist = await _passengerDbContext.Passengers.AnyAsync(x => x.PassportNumber == context.Message.PassportNumber);

        if (passengerExist)
            return;

        var passenger = Passengers.Models.Passenger.Create(SnowFlakIdGenerator.NewId(), context.Message.Name,
            context.Message.PassportNumber);

        await _passengerDbContext.AddAsync(passenger);

        await _passengerDbContext.SaveChangesAsync();

        await _eventDispatcher.SendAsync(
            new PassengerCreatedDomainEvent(passenger.Id, passenger.Name, passenger.PassportNumber),
            typeof(IInternalCommand));
    }
}
