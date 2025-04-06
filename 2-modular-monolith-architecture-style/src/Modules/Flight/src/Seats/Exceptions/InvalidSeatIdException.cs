namespace Flight.Seats.Exceptions;
using System;
using BuildingBlocks.Exception;


public class InvalidSeatIdException : BadRequestException
{
    public InvalidSeatIdException(Guid seatId)
        : base($"seatId: '{seatId}' is invalid.")
    {
    }
}
