namespace Flight.Seats.Exceptions;
using System;
using BuildingBlocks.Exception;


public class InvalidSeatIdExceptions : BadRequestException
{
    public InvalidSeatIdExceptions(Guid seatId)
        : base($"seatId: '{seatId}' is invalid.")
    {
    }
}
