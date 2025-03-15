namespace BuildingBlocks.Mongo;

public class MongoOptions
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public static Guid UniqueId { get; set; } = Guid.NewGuid();
}
