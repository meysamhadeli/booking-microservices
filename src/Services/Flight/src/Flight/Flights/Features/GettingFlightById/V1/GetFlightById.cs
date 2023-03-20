namespace Flight.Flights.Features.GettingFlightById.V1;

using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using Data;
using Dtos;
using Exceptions;
using FluentValidation;
using MapsterMapper;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

public record GetFlightById(long Id) : IQuery<FlightDto>;

internal class GetFlightByIdValidator : AbstractValidator<GetFlightById>
{
    public GetFlightByIdValidator()
    {
        RuleFor(x => x.Id).NotNull().WithMessage("Id is required!");
    }
}

internal class GetFlightByIdHandler : IQueryHandler<GetFlightById, FlightDto>
{
    private readonly IMapper _mapper;
    private readonly FlightReadDbContext _flightReadDbContext;

    public GetFlightByIdHandler(IMapper mapper, FlightReadDbContext flightReadDbContext)
    {
        _mapper = mapper;
        _flightReadDbContext = flightReadDbContext;
    }

    public async Task<FlightDto> Handle(GetFlightById request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var flight =
            await _flightReadDbContext.Flight.AsQueryable().SingleOrDefaultAsync(x => x.FlightId == request.Id &&
                !x.IsDeleted, cancellationToken);

        if (flight is null)
        {
            throw new FlightNotFountException();
        }

        return _mapper.Map<FlightDto>(flight);
    }
}
