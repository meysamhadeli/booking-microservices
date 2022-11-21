using BuildingBlocks.IdsGenerator;
using Flight.Aircrafts.Features.CreateAircraft.Commands.V1;
using Flight.Aircrafts.Features.CreateAircraft.Commands.V1.Reads;
using Flight.Aircrafts.Features.CreateAircraft.Dtos.V1;
using Flight.Aircrafts.Models;
using Flight.Aircrafts.Models.Reads;
using Flight.Airports.Features.CreateAirport.Commands.V1;
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

        config.NewConfig<CreateAircraftRequestDto, CreateAircraftCommand>()
            .ConstructUsing(x => new CreateAircraftCommand(x.Name, x.Model, x.ManufacturingYear));
    }
}
