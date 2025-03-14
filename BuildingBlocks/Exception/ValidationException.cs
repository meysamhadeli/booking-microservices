using BuildingBlocks.Validation;

namespace BuildingBlocks.Exception
{
    public class ValidationException : CustomException
    {
        public ValidationException(string message, int? code = null) : base(message, code: code)
        {
        }
    }
}
