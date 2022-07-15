using BuildingBlocks.IdsGenerator;
using BuildingBlocks.Utils;
using ICommand = BuildingBlocks.Core.CQRS.ICommand;

namespace BuildingBlocks.Core.Event;

public class InternalCommand : IInternalCommand, ICommand
{
    public long Id { get; set; } = SnowFlakIdGenerator.NewId();

    public DateTime OccurredOn => DateTime.Now;

    public string Type { get => TypeProvider.GetTypeName(GetType()); }
}
