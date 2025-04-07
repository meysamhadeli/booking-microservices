using BookingMonolith.Flight.Flights.ValueObjects;
using BookingMonolith.Flight.Seats.Features.CreatingSeat.V1;
using BookingMonolith.Flight.Seats.Features.ReservingSeat.V1;
using BookingMonolith.Flight.Seats.ValueObjects;
using BuildingBlocks.Core.Model;

namespace BookingMonolith.Flight.Seats.Models;

public record Seat : Aggregate<SeatId>
{
    public SeatNumber SeatNumber { get; private set; } = default!;
    public Enums.SeatType Type { get; private set; }
    public Enums.SeatClass Class { get; private set; }
    public FlightId FlightId { get; private set; } = default!;

    public static Seat Create(SeatId id, SeatNumber seatNumber, Enums.SeatType type, Enums.SeatClass @class,
        FlightId flightId,
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
            seat.Id,
            seat.SeatNumber,
            seat.Type,
            seat.Class,
            seat.FlightId,
            isDeleted);

        seat.AddDomainEvent(@event);

        return seat;
    }

    public void ReserveSeat()
    {
        this.IsDeleted = true;
        this.LastModified = DateTime.Now;

        var @event = new SeatReservedDomainEvent(
            this.Id,
            this.SeatNumber,
            this.Type,
            this.Class,
            this.FlightId,
            this.IsDeleted);

        this.AddDomainEvent(@event);
    }
}
