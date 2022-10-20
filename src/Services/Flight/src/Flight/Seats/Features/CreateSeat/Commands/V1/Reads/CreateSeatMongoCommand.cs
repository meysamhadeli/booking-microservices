using BuildingBlocks.Core.Event;

namespace Flight.Seats.Features.CreateSeat.Commands.V1.Reads;

public class CreateSeatMongoCommand : InternalCommand
{
    public CreateSeatMongoCommand(long id, string seatNumber, Enums.SeatType type, Enums.SeatClass @class,
        long flightId, bool isDeleted)
    {
        Id = id;
        SeatNumber = seatNumber;
        Type = type;
        Class = @class;
        FlightId = flightId;
        IsDeleted = isDeleted;
    }

    public long Id { get; }
    public string SeatNumber { get; }
    public Enums.SeatType Type { get; }
    public Enums.SeatClass Class { get; }
    public long FlightId { get; }
    public bool IsDeleted { get; }
}
