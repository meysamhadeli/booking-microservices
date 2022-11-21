using System;
using BuildingBlocks.Core.Event;

namespace Flight.Flights.Features.UpdateFlight.Commands.V1.Reads;

public record UpdateFlightMongoCommand(long Id, string FlightNumber, long AircraftId, DateTime DepartureDate,
    long DepartureAirportId, DateTime ArriveDate, long ArriveAirportId, decimal DurationMinutes, DateTime FlightDate,
    Enums.FlightStatus Status, decimal Price, bool IsDeleted) : InternalCommand;
