using System.Net;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Exception;

public class AppException : CustomException
{
    public AppException(string message, int? code = null) : base(message, code: code)
    {
    }

    public AppException() : base()
    {
    }

    public AppException(string message, HttpStatusCode statusCode, int? code = null) : base(message, statusCode, code)
    {
    }

    public AppException(string message,  System.Exception innerException, int? code = null) : base(message, innerException, code: code)
    {
    }
}
