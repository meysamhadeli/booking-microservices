using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using Flight.Data;
using Flight.Flights.Dtos;
using Flight.Flights.Exceptions;
using MapsterMapper;
using MongoDB.Driver;

namespace Flight.Flights.Features.GetAvailableFlights.Queries.V1;

public class GetAvailableFlightsQueryHandler : IQueryHandler<GetAvailableFlightsQuery, IEnumerable<FlightResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly FlightReadDbContext _flightReadDbContext;

    public GetAvailableFlightsQueryHandler(IMapper mapper, FlightReadDbContext flightReadDbContext)
    {
        _mapper = mapper;
        _flightReadDbContext = flightReadDbContext;
    }

    public async Task<IEnumerable<FlightResponseDto>> Handle(GetAvailableFlightsQuery query,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var flight = (await _flightReadDbContext.Flight.AsQueryable().ToListAsync(cancellationToken))
            .Where(x => !x.IsDeleted);

        if (!flight.Any())
            throw new FlightNotFountException();

        return _mapper.Map<List<FlightResponseDto>>(flight);
    }
}
