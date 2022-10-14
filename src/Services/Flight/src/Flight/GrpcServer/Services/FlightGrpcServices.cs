using System.Threading.Tasks;
using Flight.Flights.Features.GetFlightById;
using Flight.Seats.Features.GetAvailableSeats;
using Flight.Seats.Features.ReserveSeat;
using Grpc.Core;
using Mapster;
using MediatR;

namespace Flight.GrpcServer.Services;

public class FlightGrpcServices : FlightGrpcService.FlightGrpcServiceBase
{
    private readonly IMediator _mediator;

    public FlightGrpcServices(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<FlightResponse> GetById(GetByIdRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new GetFlightByIdQuery(request.Id));
        return result.Adapt<FlightResponse>();
    }

    public override async Task<SeatsResponse> ReserveSeat(ReserveSeatRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new ReserveSeatCommand(request.FlightId, request.SeatNumber));
        return result.Adapt<SeatsResponse>();
    }

    public override async Task<ListSeatsResponse> GetAvailableSeats(GetAvailableSeatsRequest request, ServerCallContext context)
    {
        var result = new ListSeatsResponse();

        var availableSeats = await _mediator.Send(new GetAvailableSeatsQuery(request.FlightId));

        foreach (var availableSeat in availableSeats)
        {
            result.Items.Add(availableSeat.Adapt<SeatsResponse>());
        }

        return result;
    }
}
