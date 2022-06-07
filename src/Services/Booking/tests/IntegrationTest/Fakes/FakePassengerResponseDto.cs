using AutoBogus;
using BuildingBlocks.Contracts.Grpc;
using BuildingBlocks.IdsGenerator;

namespace Integration.Test.Fakes;

public class FakePassengerResponseDto : AutoFaker<PassengerResponseDto>
{
    public FakePassengerResponseDto()
    {
        RuleFor(r => r.Id, _ => SnowFlakIdGenerator.NewId());
    }
}
