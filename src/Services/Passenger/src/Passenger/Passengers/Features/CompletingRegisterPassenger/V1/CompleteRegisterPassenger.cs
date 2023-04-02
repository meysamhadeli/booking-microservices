namespace Passenger.Passengers.Features.CompletingRegisterPassenger.V1;

using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Exceptions;
using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Data;
using Dtos;
using MassTransit;

public record CompleteRegisterPassenger
    (string PassportNumber, Enums.PassengerType PassengerType, int Age) : ICommand<CompleteRegisterPassengerResult>,
        IInternalCommand
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CompleteRegisterPassengerResult(PassengerDto PassengerDto);

internal class CompleteRegisterPassengerValidator : AbstractValidator<CompleteRegisterPassenger>
{
    public CompleteRegisterPassengerValidator()
    {
        RuleFor(x => x.PassportNumber).NotNull().WithMessage("The PassportNumber is required!");
        RuleFor(x => x.Age).GreaterThan(0).WithMessage("The Age must be greater than 0!");
        RuleFor(x => x.PassengerType).Must(p => p.GetType().IsEnum &&
                                                p == Enums.PassengerType.Baby ||
                                                p == Enums.PassengerType.Female ||
                                                p == Enums.PassengerType.Male ||
                                                p == Enums.PassengerType.Unknown)
            .WithMessage("PassengerType must be Male, Female, Baby or Unknown");
    }
}

internal class
    CompleteRegisterPassengerCommandHandler : ICommandHandler<CompleteRegisterPassenger,
        CompleteRegisterPassengerResult>
{
    private readonly IMapper _mapper;
    private readonly PassengerDbContext _passengerDbContext;

    public CompleteRegisterPassengerCommandHandler(IMapper mapper, PassengerDbContext passengerDbContext)
    {
        _mapper = mapper;
        _passengerDbContext = passengerDbContext;
    }

    public async Task<CompleteRegisterPassengerResult> Handle(CompleteRegisterPassenger request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var passenger = await _passengerDbContext.Passengers.AsNoTracking().SingleOrDefaultAsync(
            x => x.PassportNumber == request.PassportNumber, cancellationToken);

        if (passenger is null)
        {
            throw new PassengerNotExist();
        }

        var passengerEntity = passenger.CompleteRegistrationPassenger(passenger.Id, passenger.Name,
            passenger.PassportNumber, request.PassengerType, request.Age);

        var updatePassenger = _passengerDbContext.Passengers.Update(passengerEntity);

        var passengerDto = _mapper.Map<PassengerDto>(updatePassenger.Entity);

        return new CompleteRegisterPassengerResult(passengerDto);
    }
}
