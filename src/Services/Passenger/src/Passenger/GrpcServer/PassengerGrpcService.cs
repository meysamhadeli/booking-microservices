using BuildingBlocks.Contracts.Grpc;
using MagicOnion;
using MagicOnion.Server;
using Mapster;
using MediatR;
using Passenger.Passengers.Features.GetPassengerById;

namespace Passenger.GrpcServer;

public class PassengerGrpcService : ServiceBase<IPassengerGrpcService>, IPassengerGrpcService
{
    private readonly IMediator _mediator;

    public PassengerGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async UnaryResult<PassengerResponseDto> GetById(long id)
    {
        var result = await _mediator.Send(new GetPassengerQueryById(id));
        return result.Adapt<PassengerResponseDto>();
    }
}
