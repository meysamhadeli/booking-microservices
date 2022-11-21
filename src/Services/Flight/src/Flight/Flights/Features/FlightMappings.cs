using AutoMapper;
using BuildingBlocks.IdsGenerator;
using Flight.Flights.Dtos;
using Flight.Flights.Features.CreateFlight.Commands.V1;
using Flight.Flights.Features.CreateFlight.Commands.V1.Reads;
using Flight.Flights.Features.CreateFlight.Dtos.V1;
using Flight.Flights.Features.DeleteFlight.Commands.V1.Reads;
using Flight.Flights.Features.UpdateFlight.Commands.V1;
using Flight.Flights.Features.UpdateFlight.Commands.V1.Reads;
using Flight.Flights.Features.UpdateFlight.Dtos;
using Flight.Flights.Models.Reads;
using Mapster;

namespace Flight.Flights.Features;

public class FlightMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.Flight, FlightResponseDto>()
            .ConstructUsing(x => new FlightResponseDto(x.Id, x.FlightNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.FlightDate, x.Status, x.Price));

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

        config.NewConfig<CreateFlightRequestDto, CreateFlightCommand>()
            .ConstructUsing(x => new CreateFlightCommand(x.FlightNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.FlightDate, x.Status, x.Price));

        config.NewConfig<UpdateFlightRequestDto, UpdateFlightCommand>()
            .ConstructUsing(x => new UpdateFlightCommand(x.Id, x.FlightNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.FlightDate, x.Status, x.IsDeleted, x.Price));

    }
}
