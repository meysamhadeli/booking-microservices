namespace BuildingBlocks.Exception
{
    public class ConflictException : CustomException
    {
        public ConflictException(string message, string code = null) : base(message, code: code)
        {
        }
    }
}
