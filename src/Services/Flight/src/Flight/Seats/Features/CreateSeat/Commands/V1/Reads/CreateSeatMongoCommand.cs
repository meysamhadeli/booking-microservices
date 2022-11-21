using BuildingBlocks.Core.Event;

namespace Flight.Seats.Features.CreateSeat.Commands.V1.Reads;

public record CreateSeatMongoCommand(long Id, string SeatNumber, Enums.SeatType Type,
    Enums.SeatClass Class, long FlightId, bool IsDeleted) : InternalCommand;
