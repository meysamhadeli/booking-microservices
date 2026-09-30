using System.Net;

namespace Woo.Core.Exception
{
    public class DomainException : CustomException
    {
        public DomainException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            : base(message, statusCode) { }

        public DomainException(
            string message,
            System.Exception innerException,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest,
            int? code = null
        )
            : base(message, innerException, statusCode, code) { }
    }
}
