using AutoMapper;
using BuildingBlocks.IdsGenerator;
using Flight.Flights.Dtos;
using Flight.Flights.Features.CreateFlight.Commands.V1.Reads;
using Flight.Flights.Features.DeleteFlight.Commands.V1.Reads;
using Flight.Flights.Features.UpdateFlight.Commands.V1.Reads;
using Flight.Flights.Models.Reads;
using Mapster;

namespace Flight.Flights.Features;

public class FlightMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.Flight, FlightResponseDto>()
            .Map(d => d.FlightId, s => s.Id);
        config.NewConfig<CreateFlightMongoCommand, FlightReadModel>()
            .Map(d => d.Id, s => SnowFlakIdGenerator.NewId())
            .Map(d => d.FlightId, s => s.Id);
        config.NewConfig<Models.Flight, FlightReadModel>()
            .Map(d => d.Id, s => SnowFlakIdGenerator.NewId())
            .Map(d => d.FlightId, s => s.Id);
        config.NewConfig<UpdateFlightMongoCommand, FlightReadModel>()
            .Map(d => d.FlightId, s => s.Id);
        config.NewConfig<DeleteFlightMongoCommand, FlightReadModel>()
            .Map(d => d.FlightId, s => s.Id);
    }
}
