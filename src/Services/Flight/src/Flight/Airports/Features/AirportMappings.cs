namespace Flight.Airports.Features;

using CreatingAirport.V1;
using Models;
using Mapster;
using MassTransit;

public class AirportMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateAirportMongo, AirportReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.AirportId, s => s.Id);

        config.NewConfig<Airport, AirportReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.AirportId, s => s.Id);

        config.NewConfig<CreateAirportRequestDto, CreateAirport>()
            .ConstructUsing(x => new CreateAirport(x.Name, x.Address, x.Code));
    }
}
