using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using Flight.Data;
using Flight.Flights.Dtos;
using Flight.Flights.Exceptions;
using MapsterMapper;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Flight.Flights.Features.GetFlightById.Queries.V1;

public class GetFlightByIdQueryHandler : IQueryHandler<GetFlightByIdQuery, FlightResponseDto>
{
    private readonly IMapper _mapper;
    private readonly FlightReadDbContext _flightReadDbContext;

    public GetFlightByIdQueryHandler(IMapper mapper, FlightReadDbContext flightReadDbContext)
    {
        _mapper = mapper;
        _flightReadDbContext = flightReadDbContext;
    }

    public async Task<FlightResponseDto> Handle(GetFlightByIdQuery query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var flight =
            await _flightReadDbContext.Flight.AsQueryable().SingleOrDefaultAsync(x => x.FlightId == query.Id &&
                !x.IsDeleted, cancellationToken);

        if (flight is null)
            throw new FlightNotFountException();

        return _mapper.Map<FlightResponseDto>(flight);
    }
}
