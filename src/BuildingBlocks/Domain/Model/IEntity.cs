namespace BuildingBlocks.Domain.Model;

public interface IEntity
{
    long Id { get; }
}

public interface IEntity<out TId>
{
    TId Id { get; }
    public bool IsDeleted { get; }
}
