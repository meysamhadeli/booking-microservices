using BuildingBlocks.Core.Event;
using Flight.Seats.Models;

namespace Flight.Seats.Features.ReserveSeat.Reads;

public class ReserveSeatMongoCommand : InternalCommand
{
    public ReserveSeatMongoCommand(long id, string seatNumber, Enums.SeatType type, Enums.SeatClass @class, long flightId,
        bool isDeleted)
    {
        Id = id;
        SeatNumber = seatNumber;
        Type = type;
        Class = @class;
        FlightId = flightId;
        IsDeleted = isDeleted;
    }

    public string SeatNumber { get; }
    public Enums.SeatType Type { get; }
    public Enums.SeatClass Class { get; }
    public long FlightId { get; }
    public bool IsDeleted { get; }
}
