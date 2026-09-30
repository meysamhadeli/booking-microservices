using System.Net;

namespace Woo.Core.Exception;

public class AppException : CustomException
{
    public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, int? code = null) : base(message, statusCode, code: code)
    {
    }

    public AppException(string message, System.Exception innerException, HttpStatusCode statusCode = HttpStatusCode.BadRequest, int? code = null) : base(message, innerException, statusCode, code)
    {
    }
}