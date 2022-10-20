using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Passenger.Data;
using Passenger.Passengers.Models.Reads;

namespace Passenger.Passengers.Features.CompleteRegisterPassenger.Commands.V1.Reads;

public class CompleteRegisterPassengerMongoCommandHandler : ICommandHandler<CompleteRegisterPassengerMongoCommand>
{
    private readonly PassengerReadDbContext _passengerReadDbContext;
    private readonly IMapper _mapper;

    public CompleteRegisterPassengerMongoCommandHandler(
        PassengerReadDbContext passengerReadDbContext,
        IMapper mapper)
    {
        _passengerReadDbContext = passengerReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CompleteRegisterPassengerMongoCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var passengerReadModel = _mapper.Map<PassengerReadModel>(command);

        var passenger = await _passengerReadDbContext.Passenger.AsQueryable()
            .FirstOrDefaultAsync(x => x.PassengerId == command.Id && !x.IsDeleted, cancellationToken);

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
