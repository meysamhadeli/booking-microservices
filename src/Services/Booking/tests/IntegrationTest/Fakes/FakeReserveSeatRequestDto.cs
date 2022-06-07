using AutoBogus;
using BuildingBlocks.Contracts.Grpc;

namespace Integration.Test.Fakes;

public class FakeReserveSeatRequestDto : AutoFaker<ReserveSeatRequestDto>
{
    public FakeReserveSeatRequestDto()
    {
        RuleFor(r => r.FlightId, _ => 1);
        RuleFor(r => r.SeatNumber, _ => "33F");
    }
}
