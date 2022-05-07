using Ardalis.GuardClauses;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Passenger.Data;
using Passenger.Passengers.Dtos;
using Passenger.Passengers.Exceptions;

namespace Passenger.Passengers.Features.CompleteRegisterPassenger;

public class CompleteRegisterPassengerCommandHandler : IRequestHandler<CompleteRegisterPassengerCommand, PassengerResponseDto>
{
    private readonly IMapper _mapper;
    private readonly PassengerDbContext _passengerDbContext;

    public CompleteRegisterPassengerCommandHandler(IMapper mapper, PassengerDbContext passengerDbContext)
    {
        _mapper = mapper;
        _passengerDbContext = passengerDbContext;
    }

    public async Task<PassengerResponseDto> Handle(CompleteRegisterPassengerCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var passenger = await _passengerDbContext.Passengers.AsNoTracking().SingleOrDefaultAsync(
            x => x.PassportNumber == command.PassportNumber,
            cancellationToken);

        if (passenger is null)
            throw new PassengerNotExist();

        var passengerEntity = passenger.CompleteRegistrationPassenger(passenger.Id, passenger.Name, passenger.PassportNumber, command.PassengerType, command.Age);

        var updatePassenger = _passengerDbContext.Passengers.Update(passengerEntity);

        return _mapper.Map<PassengerResponseDto>(updatePassenger.Entity);
    }
}
