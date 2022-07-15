using BuildingBlocks.IdsGenerator;
using Flight.Airports.Features.CreateAirport.Reads;
using Flight.Airports.Models.Reads;
using Mapster;

namespace Flight.Airports;

public class AirportMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateAirportMongoCommand, AirportReadModel>()
            .Map(d => d.Id, s => SnowFlakIdGenerator.NewId())
            .Map(d => d.AirportId, s => s.Id);
    }
}
