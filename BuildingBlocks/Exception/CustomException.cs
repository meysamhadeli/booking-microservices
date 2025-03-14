using System.Net;

namespace BuildingBlocks.Exception;

public class CustomException : System.Exception
{
    public CustomException(
        string message,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        int? code = null) : base(message)
    {
        StatusCode = statusCode;
        Code = code;
    }

    public CustomException(
        string message,
        System.Exception innerException,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        int? code = null) : base(message, innerException)
    {
        StatusCode = statusCode;
        Code = code;
    }

    public CustomException(
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        int? code = null) : base()
    {
        StatusCode = statusCode;
        Code = code;
    }

    public HttpStatusCode StatusCode { get; }

    public int? Code { get; }
}
