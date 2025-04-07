using BookingMonolith.Flight.Flights.Features.CreatingFlight.V1;
using BookingMonolith.Flight.Flights.Features.DeletingFlight.V1;
using BookingMonolith.Flight.Flights.Features.UpdatingFlight.V1;
using BookingMonolith.Flight.Flights.Models;
using Mapster;
using MassTransit;

namespace BookingMonolith.Flight.Flights.Features;

using FlightDto = Dtos.FlightDto;

public class FlightMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.Flight, FlightDto>()
            .ConstructUsing(x => new FlightDto(x.Id, x.FlightNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.FlightDate, x.Status, x.Price));

        config.NewConfig<CreateFlightMongo, FlightReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.FlightId, s => s.Id);

        config.NewConfig<Models.Flight, FlightReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.FlightId, s => s.Id.Value);

        config.NewConfig<FlightReadModel, FlightDto>()
            .Map(d => d.Id, s => s.FlightId);

        config.NewConfig<UpdateFlightMongo, FlightReadModel>()
            .Map(d => d.FlightId, s => s.Id);

        config.NewConfig<DeleteFlightMongo, FlightReadModel>()
            .Map(d => d.FlightId, s => s.Id);

        config.NewConfig<CreateFlightRequestDto, CreateFlight>()
            .ConstructUsing(x => new CreateFlight(x.FlightNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.FlightDate, x.Status, x.Price));

        config.NewConfig<UpdateFlightRequestDto, UpdateFlight>()
            .ConstructUsing(x => new UpdateFlight(x.Id, x.FlightNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.FlightDate, x.Status, x.IsDeleted, x.Price));

    }
}
