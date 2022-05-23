using System;

namespace BuildingBlocks.Exception
{
    public class BadRequestException : CustomException
    {
        public BadRequestException(string message, string code = null) : base(message, code: code)
        {

        }
    }
}
