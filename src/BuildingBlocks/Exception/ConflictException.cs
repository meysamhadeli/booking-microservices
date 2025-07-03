using System.Net;

namespace BuildingBlocks.Exception
{
    public class ConflictException : CustomException
    {
        public ConflictException(string message, int? code = null) : base(message, HttpStatusCode.Conflict, code: code)
        {
        }
    }
}
