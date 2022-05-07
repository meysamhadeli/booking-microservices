using System.Net;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Exception;

public class AppException : CustomException
{
    public AppException(string message, string code = default!) : base(message)
    {
        Code = code;
    }

    public AppException() : base()
    {
    }

    public AppException(string message) : base(message)
    {
    }

    public AppException(string message, HttpStatusCode statusCode) : base(message, statusCode)
    {
    }

    public AppException(string message,  System.Exception innerException) : base(message, innerException)
    {
    }

    public string Code { get; }
}
