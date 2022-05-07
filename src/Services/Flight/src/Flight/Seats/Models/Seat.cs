using System;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Model;

namespace Flight.Seats.Models;

public class Seat : Aggregate<long>
{
    public static Seat Create(long id, string seatNumber, SeatType type, SeatClass @class, long flightId)
    {
        var seat = new Seat()
        {
            Id = id,
            Class = @class,
            Type = type,
            SeatNumber = seatNumber,
            FlightId = flightId
        };

        return seat;
    }

    public Task<Seat> ReserveSeat(Seat seat)
    {
        seat.IsDeleted = true;
        seat.LastModified = DateTime.Now;
        return Task.FromResult(this);
    }

    public string SeatNumber { get; private set; }
    public SeatType Type { get; private set; }
    public SeatClass Class { get; private set; }
    public long FlightId { get; private set; }
}
