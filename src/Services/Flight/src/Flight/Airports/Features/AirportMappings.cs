namespace Flight.Airports.Features;

using BuildingBlocks.IdsGenerator;
using Flight.Airports.Features.CreatingAirport.V1;
using Flight.Airports.Models;
using Mapster;

public class AirportMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateAirportMongo, AirportReadModel>()
            .Map(d => d.Id, s => SnowflakeIdGenerator.NewId())
            .Map(d => d.AirportId, s => s.Id);

        config.NewConfig<Airport, AirportReadModel>()
            .Map(d => d.Id, s => SnowflakeIdGenerator.NewId())
            .Map(d => d.AirportId, s => s.Id);

        config.NewConfig<CreateAirportRequestDto, CreateAirport>()
            .ConstructUsing(x => new CreateAirport(x.Name, x.Address, x.Code));
    }
}
