namespace BuildingBlocks.Exception
{
    public class ConflictException : CustomException
    {
        public ConflictException(string message, int? code = null) : base(message, code: code)
        {
        }
    }
}
