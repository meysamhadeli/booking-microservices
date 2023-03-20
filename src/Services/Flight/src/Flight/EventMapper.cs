using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Core;
using BuildingBlocks.Core.Event;

namespace Flight;

using Aircrafts.Features.CreatingAircraft.V1;
using Airports.Features.CreatingAirport.V1;
using Flights.Features.CreatingFlight.V1;
using Flights.Features.DeletingFlight.V1;
using Flights.Features.UpdatingFlight.V1;
using Seats.Features.CreatingSeat.V1;
using Seats.Features.ReservingSeat.Commands.V1;

// ref: https://www.ledjonbehluli.com/posts/domain_to_integration_event/
public sealed class EventMapper : IEventMapper
{
    public IIntegrationEvent MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            FlightCreatedDomainEvent e => new FlightCreated(e.Id),
            FlightUpdatedDomainEvent e => new FlightUpdated(e.Id),
            FlightDeletedDomainEvent e => new FlightDeleted(e.Id),
            AirportCreatedDomainEvent e => new AirportCreated(e.Id),
            AircraftCreatedDomainEvent e => new AircraftCreated(e.Id),
            SeatCreatedDomainEvent e => new SeatCreated(e.Id),
            SeatReservedDomainEvent e => new SeatReserved(e.Id),
            _ => null
        };
    }

    public IInternalCommand MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            FlightCreatedDomainEvent e => new CreateFlightMongo(e.Id, e.FlightNumber, e.AircraftId, e.DepartureDate, e.DepartureAirportId,
                e.ArriveDate, e.ArriveAirportId, e.DurationMinutes, e.FlightDate, e.Status, e.Price, e.IsDeleted),
            FlightUpdatedDomainEvent e => new UpdateFlightMongo(e.Id, e.FlightNumber, e.AircraftId, e.DepartureDate, e.DepartureAirportId,
                e.ArriveDate, e.ArriveAirportId, e.DurationMinutes, e.FlightDate, e.Status, e.Price, e.IsDeleted),
            FlightDeletedDomainEvent e => new DeleteFlightMongo(e.Id, e.FlightNumber, e.AircraftId, e.DepartureDate, e.DepartureAirportId,
                e.ArriveDate, e.ArriveAirportId, e.DurationMinutes, e.FlightDate, e.Status, e.Price, e.IsDeleted),
            AircraftCreatedDomainEvent e => new CreateAircraftMongo(e.Id, e.Name, e.Model, e.ManufacturingYear, e.IsDeleted),
            AirportCreatedDomainEvent e => new CreateAirportMongo(e.Id, e.Name, e.Address, e.Code, e.IsDeleted),
            SeatCreatedDomainEvent e => new CreateSeatMongo(e.Id, e.SeatNumber, e.Type, e.Class, e.FlightId, e.IsDeleted),
            SeatReservedDomainEvent e => new ReserveSeatMongo(e.Id, e.SeatNumber, e.Type, e.Class, e.FlightId, e.IsDeleted),
            _ => null
        };
    }
}
