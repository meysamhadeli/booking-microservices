using BookingMonolith.Flight.Aircrafts.Features.CreatingAircraft.V1;
using BookingMonolith.Flight.Aircrafts.Models;
using BookingMonolith.Flight.Aircrafts.ValueObjects;
using Mapster;
using MassTransit;

namespace BookingMonolith.Flight.Aircrafts.Features;

public class AircraftMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateAircraftMongo, AircraftReadModel>()
        .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.AircraftId, s => AircraftId.Of(s.Id));

        config.NewConfig<Aircraft, AircraftReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.AircraftId, s => AircraftId.Of(s.Id.Value));

        config.NewConfig<CreateAircraftRequestDto, CreatingAircraft.V1.CreateAircraft>()
            .ConstructUsing(x => new CreatingAircraft.V1.CreateAircraft(x.Name, x.Model, x.ManufacturingYear));
    }
}
