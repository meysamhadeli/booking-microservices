using System.Threading.Tasks;
using Grpc.Core;
using Mapster;
using MediatR;

namespace Flight.GrpcServer.Services;

using Flights.Features.GettingFlightById.V1;
using Seats.Features.GettingAvailableSeats.V1;
using Seats.Features.ReservingSeat.Commands.V1;

public class FlightGrpcServices : FlightGrpcService.FlightGrpcServiceBase
{
    private readonly IMediator _mediator;

    public FlightGrpcServices(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<FlightResponse> GetById(GetByIdRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new GetFlightById(request.Id));
        return result.Adapt<FlightResponse>();
    }

    public override async Task<SeatsResponse> ReserveSeat(ReserveSeatRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new ReserveSeat(request.FlightId, request.SeatNumber));
        return result.Adapt<SeatsResponse>();
    }

    public override async Task<ListSeatsResponse> GetAvailableSeats(GetAvailableSeatsRequest request, ServerCallContext context)
    {
        var result = new ListSeatsResponse();

        var availableSeats = await _mediator.Send(new GetAvailableSeats(request.FlightId));

        foreach (var availableSeat in availableSeats)
        {
            result.Items.Add(availableSeat.Adapt<SeatsResponse>());
        }

        return result;
    }
}
