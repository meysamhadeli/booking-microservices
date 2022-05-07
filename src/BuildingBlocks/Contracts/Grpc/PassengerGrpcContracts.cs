using MagicOnion;
using MessagePack;

namespace BuildingBlocks.Contracts.Grpc;

public interface IPassengerGrpcService : IService<IPassengerGrpcService>
{
    UnaryResult<PassengerResponseDto> GetById(long id);
}


[MessagePackObject]
public class PassengerResponseDto
{
    [Key(0)]
    public long Id { get; init; }
    [Key(1)]
    public string Name { get; init; }
    [Key(2)]
    public string PassportNumber { get; init; }
    [Key(3)]
    public PassengerType PassengerType { get; init; }
    [Key(4)]
    public int Age { get; init; }
    [Key(5)]
    public string Email { get; init; }
}

public enum PassengerType
{
    Male,
    Female,
    Baby,
    Unknown
}
