using System;

namespace BuildingBlocks.Exception
{
    public class BadRequestException : CustomException
    {
        public BadRequestException(string message) : base(message)
        {

        }
    }
}