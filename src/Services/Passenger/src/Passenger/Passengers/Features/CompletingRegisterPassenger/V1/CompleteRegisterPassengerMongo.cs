namespace Passenger.Passengers.Features.CompletingRegisterPassenger.V1;

using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using MapsterMapper;
using MediatR;
using Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Data;

public record CompleteRegisterPassengerMongoCommand(Guid Id, string PassportNumber, string Name,
    Enums.PassengerType PassengerType, int Age, bool IsDeleted) : InternalCommand;


internal class CompleteRegisterPassengerMongoHandler : ICommandHandler<CompleteRegisterPassengerMongoCommand>
{
    private readonly PassengerReadDbContext _passengerReadDbContext;
    private readonly IMapper _mapper;

    public CompleteRegisterPassengerMongoHandler(
        PassengerReadDbContext passengerReadDbContext,
        IMapper mapper)
    {
        _passengerReadDbContext = passengerReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CompleteRegisterPassengerMongoCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var passengerReadModel = _mapper.Map<PassengerReadModel>(request);

        var passenger = await _passengerReadDbContext.Passenger.AsQueryable()
            .FirstOrDefaultAsync(x => x.PassengerId == passengerReadModel.PassengerId && !x.IsDeleted, cancellationToken);

        if (passenger is not null)
        {
            await _passengerReadDbContext.Passenger.UpdateOneAsync(
                x => x.PassengerId == passengerReadModel.PassengerId,
                Builders<PassengerReadModel>.Update
                    .Set(x => x.PassengerId, passengerReadModel.PassengerId)
                    .Set(x => x.Age, passengerReadModel.Age)
                    .Set(x => x.Name, passengerReadModel.Name)
                    .Set(x => x.IsDeleted, passengerReadModel.IsDeleted)
                    .Set(x => x.PassengerType, passengerReadModel.PassengerType)
                    .Set(x => x.PassportNumber, passengerReadModel.PassportNumber),
                cancellationToken: cancellationToken);
        }
        else
        {
            await _passengerReadDbContext.Passenger.InsertOneAsync(passengerReadModel,
                cancellationToken: cancellationToken);
        }

        return Unit.Value;
    }
}
