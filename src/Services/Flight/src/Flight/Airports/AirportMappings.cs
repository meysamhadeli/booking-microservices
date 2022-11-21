using BuildingBlocks.IdsGenerator;
using Flight.Airports.Features.CreateAirport.Commands.V1;
using Flight.Airports.Features.CreateAirport.Commands.V1.Reads;
using Flight.Airports.Features.CreateAirport.Dtos.V1;
using Flight.Airports.Models;
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

        config.NewConfig<Airport, AirportReadModel>()
            .Map(d => d.Id, s => SnowFlakIdGenerator.NewId())
            .Map(d => d.AirportId, s => s.Id);

        config.NewConfig<CreateAirportRequestDto, CreateAirportCommand>()
            .ConstructUsing(x => new CreateAirportCommand(x.Name, x.Address, x.Code));
    }
}
