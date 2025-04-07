using BookingMonolith.Flight.Seats.Dtos;
using BookingMonolith.Flight.Seats.Features.CreatingSeat.V1;
using BookingMonolith.Flight.Seats.Features.ReservingSeat.V1;
using BookingMonolith.Flight.Seats.Models;
using Mapster;
using MassTransit;

namespace BookingMonolith.Flight.Seats.Features;

public class SeatMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Seat, SeatDto>()
            .ConstructUsing(x => new SeatDto(x.Id.Value, x.SeatNumber.Value, x.Type, x.Class, x.FlightId.Value));

        config.NewConfig<CreateSeatMongo, SeatReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.SeatId, s => s.Id);

        config.NewConfig<Seat, SeatReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.SeatId, s => s.Id.Value);

        config.NewConfig<ReserveSeatMongo, SeatReadModel>()
            .Map(d => d.SeatId, s => s.Id);

        config.NewConfig<CreateSeatRequestDto, CreateSeat>()
            .ConstructUsing(x => new CreateSeat(x.SeatNumber, x.Type, x.Class, x.FlightId));

        config.NewConfig<ReserveSeatRequestDto, ReserveSeat>()
            .ConstructUsing(x => new ReserveSeat(x.FlightId, x.SeatNumber));
    }
}
