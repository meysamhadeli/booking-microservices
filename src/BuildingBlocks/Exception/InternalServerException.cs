using System;
using System.Globalization;

namespace BuildingBlocks.Exception
{
    public class InternalServerException : CustomException
    {
        public InternalServerException() : base() { }

        public InternalServerException(string message) : base(message) { }

        public InternalServerException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}