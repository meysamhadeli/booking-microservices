using BuildingBlocks.IdsGenerator;
using Flight.Aircrafts.Features.CreateAircraft.Reads;
using Flight.Aircrafts.Models;
using Flight.Aircrafts.Models.Reads;
using Mapster;

namespace Flight.Aircrafts.Features;

public class AircraftMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateAircraftMongoCommand, AircraftReadModel>()
            .Map(d => d.Id, s => SnowFlakIdGenerator.NewId())
            .Map(d => d.AircraftId, s => s.Id);

        config.NewConfig<Aircraft, AircraftReadModel>()
            .Map(d => d.Id, s => SnowFlakIdGenerator.NewId())
            .Map(d => d.AircraftId, s => s.Id);
    }
}
