using BuildingBlocks.IdsGenerator;
using BuildingBlocks.Utils;

namespace BuildingBlocks.Core.Event;

public class InternalCommand : IInternalCommand
{
    public long Id { get; set; } = SnowFlakIdGenerator.NewId();

    public DateTime OccurredOn => DateTime.Now;

    public string Type { get => TypeProvider.GetTypeName(GetType()); }
}
