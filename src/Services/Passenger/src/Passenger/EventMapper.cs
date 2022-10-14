using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Core;
using BuildingBlocks.Core.Event;
using Passenger.Passengers.Events.Domain;
using Passenger.Passengers.Features.CompleteRegisterPassenger.Reads;

namespace Passenger;

public sealed class EventMapper : IEventMapper
{
    public IIntegrationEvent MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            PassengerRegistrationCompletedDomainEvent e => new PassengerRegistrationCompleted(e.Id),
            PassengerCreatedDomainEvent e => new PassengerCreated(e.Id),
            _ => null
        };
    }

    public IInternalCommand MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            PassengerRegistrationCompletedDomainEvent e => new CompleteRegisterPassengerMongoCommand(e.Id, e.PassportNumber, e.Name, e.PassengerType,
                e.Age, e.IsDeleted),
            PassengerCreatedDomainEvent e => new CompleteRegisterPassengerMongoCommand(e.Id, e.PassportNumber, e.Name, Passengers.Enums.PassengerType.Unknown,
                0, e.IsDeleted),
            _ => null
        };
    }
}
