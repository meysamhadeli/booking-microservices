using BuildingBlocks.Core.Event;

namespace Flight.Seats.Features.ReserveSeat.Commands.V1.Reads;

public record ReserveSeatMongoCommand(long Id, string SeatNumber, Enums.SeatType Type,
    Enums.SeatClass Class, long FlightId, bool IsDeleted) : InternalCommand;
