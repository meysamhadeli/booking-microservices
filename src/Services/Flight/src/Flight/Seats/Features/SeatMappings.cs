using BuildingBlocks.IdsGenerator;
using Flight.Seats.Dtos;
using Flight.Seats.Features.CreateSeat.Reads;
using Flight.Seats.Features.ReserveSeat.Reads;
using Flight.Seats.Models;
using Flight.Seats.Models.Reads;
using Mapster;

namespace Flight.Seats.Features;

public class SeatMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Seat, SeatResponseDto>();
        config.NewConfig<CreateSeatMongoCommand, SeatReadModel>()
            .Map(d => d.Id, s => SnowFlakIdGenerator.NewId())
            .Map(d => d.SeatId, s => s.Id);
        config.NewConfig<Seat, SeatReadModel>()
            .Map(d => d.Id, s => SnowFlakIdGenerator.NewId())
            .Map(d => d.SeatId, s => s.Id);
        config.NewConfig<ReserveSeatMongoCommand, SeatReadModel>()
            .Map(d => d.SeatId, s => s.Id);
    }
}
