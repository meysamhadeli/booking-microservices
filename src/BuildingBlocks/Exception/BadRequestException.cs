using System;
using System.Net;

namespace BuildingBlocks.Exception
{
    public class BadRequestException : CustomException
    {
        public BadRequestException(string message, int? code = null) : base(message, HttpStatusCode.BadRequest, code: code)
        {

        }
    }
}