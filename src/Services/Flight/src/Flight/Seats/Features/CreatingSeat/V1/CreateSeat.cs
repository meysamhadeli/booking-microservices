namespace Flight.Seats.Features.CreatingSeat.V1;

using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using BuildingBlocks.IdsGenerator;
using Flight.Data;
using Flight.Seats.Dtos;
using Flight.Seats.Exceptions;
using Flight.Seats.Models;
using FluentValidation;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record CreateSeat(string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class, long FlightId) : ICommand<CreateSeatResult>, IInternalCommand
{
    public long Id { get; init; } = SnowflakeIdGenerator.NewId();
}

public record CreateSeatResult(long Id);

internal class CreateSeatValidator : AbstractValidator<CreateSeat>
{
    public CreateSeatValidator()
    {
        RuleFor(x => x.SeatNumber).NotEmpty().WithMessage("SeatNumber is required");
        RuleFor(x => x.FlightId).NotEmpty().WithMessage("FlightId is required");
        RuleFor(x => x.Class).Must(p => (p.GetType().IsEnum &&
                                         p == Enums.SeatClass.FirstClass) ||
                                        p == Enums.SeatClass.Business ||
                                        p == Enums.SeatClass.Economy)
            .WithMessage("Status must be FirstClass, Business or Economy");
    }
}

internal class CreateSeatCommandHandler : IRequestHandler<CreateSeat, CreateSeatResult>
{
    private readonly FlightDbContext _flightDbContext;

    public CreateSeatCommandHandler(FlightDbContext flightDbContext)
    {
        _flightDbContext = flightDbContext;
    }

    public async Task<CreateSeatResult> Handle(CreateSeat command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var seat = await _flightDbContext.Seats.SingleOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (seat is not null)
        {
            throw new SeatAlreadyExistException();
        }

        var seatEntity = Seat.Create(command.Id, command.SeatNumber, command.Type, command.Class, command.FlightId);

        var newSeat = (await _flightDbContext.Seats.AddAsync(seatEntity, cancellationToken))?.Entity;

        return new CreateSeatResult(newSeat.Id);
    }
}

