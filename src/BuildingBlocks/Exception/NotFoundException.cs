namespace BuildingBlocks.Exception
{
    public class NotFoundException : CustomException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}