using Grpc.Core;
using Mapster;
using MediatR;

namespace Passenger.GrpcServer.Services;

using Passengers.Features.GettingPassengerById.Queries.V1;

public class PassengerGrpcServices : PassengerGrpcService.PassengerGrpcServiceBase
{
    private readonly IMediator _mediator;

    public PassengerGrpcServices(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<PassengerResponse> GetById(GetByIdRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new GetPassengerById(request.Id));
        return result.Adapt<PassengerResponse>();
    }
}
