using BookingMonolith.Flight.Airports.Features.CreatingAirport.V1;
using BookingMonolith.Flight.Airports.Models;
using Mapster;
using MassTransit;

namespace BookingMonolith.Flight.Airports.Features;

public class AirportMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateAirportMongo, AirportReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.AirportId, s => s.Id);

        config.NewConfig<Airport, AirportReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.AirportId, s => s.Id.Value);

        config.NewConfig<CreateAirportRequestDto, CreateAirport>()
            .ConstructUsing(x => new CreateAirport(x.Name, x.Address, x.Code));
    }
}
