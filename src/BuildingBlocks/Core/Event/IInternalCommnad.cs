using BuildingBlocks.IdsGenerator;
using BuildingBlocks.Utils;

public interface IInternalCommand
{
    public long Id => SnowFlakIdGenerator.NewId();

    public DateTime OccurredOn => DateTime.Now;

    public string Type => TypeProvider.GetTypeName(GetType());
}
