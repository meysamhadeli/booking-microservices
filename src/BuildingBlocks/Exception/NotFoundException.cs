namespace BuildingBlocks.Exception
{
    public class NotFoundException : CustomException
    {
        public NotFoundException(string message, string code = null) : base(message, code: code)
        {
        }
    }
}
