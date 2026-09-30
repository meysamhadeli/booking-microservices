using System.Net;
using Woo.Core.Exception;

namespace Flight.Seats.Exceptions;

public class SeatAlreadyExistException : AppException
{
    public SeatAlreadyExistException(int? code = default) : base("Seat already exist!", HttpStatusCode.Conflict, code)
    {
    }
}