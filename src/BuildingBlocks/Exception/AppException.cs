using System.Net;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Exception;

public class AppException : CustomException
{
    public AppException(string message, string code = null) : base(message, code: code)
    {
    }

    public AppException() : base()
    {
    }

    public AppException(string message, HttpStatusCode statusCode, string code = null) : base(message, statusCode, code)
    {
    }

    public AppException(string message,  System.Exception innerException, string code = null) : base(message, innerException, code: code)
    {
    }
}
