using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Core;
using BuildingBlocks.Core.Event;
using Flight.Aircrafts.Events;
using Flight.Aircrafts.Features.CreateAircraft.Reads;
using Flight.Airports.Events;
using Flight.Airports.Features.CreateAirport.Reads;
using Flight.Flights.Events.Domain;
using Flight.Flights.Features.CreateFlight.Reads;
using Flight.Flights.Features.DeleteFlight.Reads;
using Flight.Flights.Features.UpdateFlight.Reads;
using Flight.Seats.Events;
using Flight.Seats.Features.CreateSeat.Reads;
using Flight.Seats.Features.ReserveSeat.Reads;

namespace Flight;

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
            FlightCreatedDomainEvent e => new CreateFlightMongoCommand(e.Id, e.FlightNumber, e.AircraftId, e.DepartureDate, e.DepartureAirportId,
                e.ArriveDate, e.ArriveAirportId, e.DurationMinutes, e.FlightDate, e.Status, e.Price, e.IsDeleted),
            FlightUpdatedDomainEvent e => new UpdateFlightMongoCommand(e.Id, e.FlightNumber, e.AircraftId, e.DepartureDate, e.DepartureAirportId,
                e.ArriveDate, e.ArriveAirportId, e.DurationMinutes, e.FlightDate, e.Status, e.Price, e.IsDeleted),
            FlightDeletedDomainEvent e => new DeleteFlightMongoCommand(e.Id, e.FlightNumber, e.AircraftId, e.DepartureDate, e.DepartureAirportId,
                e.ArriveDate, e.ArriveAirportId, e.DurationMinutes, e.FlightDate, e.Status, e.Price, e.IsDeleted),
            AircraftCreatedDomainEvent e => new CreateAircraftMongoCommand(e.Id, e.Name, e.Model, e.ManufacturingYear, e.IsDeleted),
            AirportCreatedDomainEvent e => new CreateAirportMongoCommand(e.Id, e.Name, e.Address, e.Code, e.IsDeleted),
            SeatCreatedDomainEvent e => new CreateSeatMongoCommand(e.Id, e.SeatNumber, e.Type, e.Class, e.FlightId, e.IsDeleted),
            SeatReservedDomainEvent e => new ReserveSeatMongoCommand(e.Id, e.SeatNumber, e.Type, e.Class, e.FlightId, e.IsDeleted),
            _ => null
        };
    }
}
