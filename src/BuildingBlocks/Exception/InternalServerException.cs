using System;
using System.Globalization;
using System.Net;

namespace BuildingBlocks.Exception
{
    public class InternalServerException : CustomException
    {
        public InternalServerException() : base() { }

        public InternalServerException(string message, int? code) : base(message, HttpStatusCode.InternalServerError, code: code) { }

        public InternalServerException(string message, int? code = null, params object[] args)
            : base(message:String.Format(CultureInfo.CurrentCulture, message, args, HttpStatusCode.InternalServerError, code))
        {
        }
    }
}
