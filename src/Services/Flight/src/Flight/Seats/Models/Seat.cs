using System;
using System.Threading.Tasks;
using BuildingBlocks.Core.Model;

namespace Flight.Seats.Models;

using Features.CreatingSeat.V1;
using Features.ReservingSeat.Commands.V1;
using Flight.Flights.ValueObjects;
using Flight.Seats.ValueObjects;

public record Seat : Aggregate<SeatId>
{
    public SeatNumber SeatNumber { get; private set; } = default!;
    public Enums.SeatType Type { get; private set; }
    public Enums.SeatClass Class { get; private set; }
    public FlightId FlightId { get; private set; } = default!;

    public static Seat Create(SeatId id, SeatNumber seatNumber, Enums.SeatType type, Enums.SeatClass @class, FlightId flightId,
        bool isDeleted = false)
    {
        var seat = new Seat()
        {
            Id = id,
            Class = @class,
            Type = type,
            SeatNumber = seatNumber,
            FlightId = flightId,
            IsDeleted = isDeleted
        };

        var @event = new SeatCreatedDomainEvent(
            seat.Id.Value,
            seat.SeatNumber.Value,
            seat.Type,
            seat.Class,
            seat.FlightId.Value,
            isDeleted);

        seat.AddDomainEvent(@event);

        return seat;
    }

    public Task<Seat> ReserveSeat(Seat seat)
    {
        seat.IsDeleted = true;
        seat.LastModified = DateTime.Now;

        var @event = new SeatReservedDomainEvent(
            seat.Id.Value,
            seat.SeatNumber.Value,
            seat.Type,
            seat.Class,
            seat.FlightId.Value,
            seat.IsDeleted);

        seat.AddDomainEvent(@event);

        return Task.FromResult(this);
    }
}
