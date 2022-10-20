using BuildingBlocks.Exception;

namespace Passenger.Passengers.Features.CompleteRegisterPassenger.Exceptions;

public class PassengerNotExist : BadRequestException
{
    public PassengerNotExist(string code = default) : base("Please register before!")
    {
    }
}
