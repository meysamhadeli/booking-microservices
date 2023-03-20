namespace Passenger.Passengers.Features.GettingPassengerById.Queries.V1;

using BuildingBlocks.Core.CQRS;
using Data;
using Dtos;
using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Ardalis.GuardClauses;
using Exceptions;

public record GetPassengerById(long Id) : IQuery<PassengerDto>;

internal class GetPassengerByIdValidator: AbstractValidator<GetPassengerById>
{
    public GetPassengerByIdValidator()
    {
        RuleFor(x => x.Id).NotNull().WithMessage("Id is required!");
    }
}

internal class GetPassengerByIdHandler : IQueryHandler<GetPassengerById, PassengerDto>
{
    private readonly PassengerDbContext _passengerDbContext;
    private readonly IMapper _mapper;

    public GetPassengerByIdHandler(IMapper mapper, PassengerDbContext passengerDbContext)
    {
        _mapper = mapper;
        _passengerDbContext = passengerDbContext;
    }

    public async Task<PassengerDto> Handle(GetPassengerById query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var passenger =
            await _passengerDbContext.Passengers.SingleOrDefaultAsync(x => x.Id == query.Id, cancellationToken);

        if (passenger is null)
        {
            throw new PassengerNotFoundException();
        }

        return _mapper.Map<PassengerDto>(passenger!);
    }
}
