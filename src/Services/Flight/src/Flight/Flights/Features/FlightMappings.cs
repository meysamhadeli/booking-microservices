using AutoMapper;
using BuildingBlocks.IdsGenerator;
using Flight.Flights.Dtos;
using Flight.Flights.Features.CreateFlight.Reads;
using Flight.Flights.Features.DeleteFlight.Reads;
using Flight.Flights.Features.UpdateFlight.Reads;
using Flight.Flights.Models.Reads;
using Mapster;

namespace Flight.Flights.Features;

public class FlightMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.Flight, FlightResponseDto>();
        config.NewConfig<CreateFlightMongoCommand, FlightReadModel>()
            .Map(d => d.Id, s => SnowFlakIdGenerator.NewId())
            .Map(d => d.FlightId, s => s.Id);
        config.NewConfig<UpdateFlightMongoCommand, FlightReadModel>()
            .Map(d => d.FlightId, s => s.Id);
        config.NewConfig<DeleteFlightMongoCommand, FlightReadModel>()
            .Map(d => d.FlightId, s => s.Id);
    }
}
