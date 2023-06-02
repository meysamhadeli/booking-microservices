namespace Flight.Aircrafts.Exceptions;
using BuildingBlocks.Exception;


public class InvalidModelException : BadRequestException
{
    public InvalidModelException() : base("Model cannot be empty or whitespace.")
    {
    }
}
