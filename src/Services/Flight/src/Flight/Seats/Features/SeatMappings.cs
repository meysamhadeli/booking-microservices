using BuildingBlocks.IdsGenerator;
using Flight.Seats.Dtos;
using Flight.Seats.Features.CreateSeat.Commands.V1;
using Flight.Seats.Features.CreateSeat.Commands.V1.Reads;
using Flight.Seats.Features.CreateSeat.Dtos.V1;
using Flight.Seats.Features.ReserveSeat.Commands.V1;
using Flight.Seats.Features.ReserveSeat.Commands.V1.Reads;
using Flight.Seats.Features.ReserveSeat.Dtos.V1;
using Flight.Seats.Models;
using Flight.Seats.Models.Reads;
using Mapster;

namespace Flight.Seats.Features;

public class SeatMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Seat, SeatResponseDto>()
            .ConstructUsing(x => new SeatResponseDto(x.Id, x.SeatNumber, x.Type, x.Class, x.FlightId));

        config.NewConfig<CreateSeatMongoCommand, SeatReadModel>()
            .Map(d => d.Id, s => SnowFlakIdGenerator.NewId())
            .Map(d => d.SeatId, s => s.Id);

        config.NewConfig<Seat, SeatReadModel>()
            .Map(d => d.Id, s => SnowFlakIdGenerator.NewId())
            .Map(d => d.SeatId, s => s.Id);

        config.NewConfig<ReserveSeatMongoCommand, SeatReadModel>()
            .Map(d => d.SeatId, s => s.Id);

        config.NewConfig<CreateSeatRequestDto, CreateSeatCommand>()
            .ConstructUsing(x => new CreateSeatCommand(x.SeatNumber, x.Type, x.Class, x.FlightId));

        config.NewConfig<ReserveSeatRequestDto, ReserveSeatCommand>()
            .ConstructUsing(x => new ReserveSeatCommand(x.FlightId, x.SeatNumber));
    }
}
