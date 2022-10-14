using Grpc.Core;
using Mapster;
using MediatR;
using Passenger.Passengers.Features.GetPassengerById;

namespace Passenger.GrpcServer.Services;

public class PassengerGrpcServices : PassengerGrpcService.PassengerGrpcServiceBase
{
    private readonly IMediator _mediator;

    public PassengerGrpcServices(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<PassengerResponse> GetById(GetByIdRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new GetPassengerQueryById(request.Id));
        return result.Adapt<PassengerResponse>();
    }
}
