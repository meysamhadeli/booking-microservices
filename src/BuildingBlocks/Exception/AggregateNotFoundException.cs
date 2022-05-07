namespace BuildingBlocks.Exception;

public class AggregateNotFoundException : System.Exception
{
    public AggregateNotFoundException(string typeName, long id): base($"{typeName} with id '{id}' was not found")
    {

    }

    public static AggregateNotFoundException For<T>(long id)
    {
        return new AggregateNotFoundException(typeof(T).Name, id);
    }
}
