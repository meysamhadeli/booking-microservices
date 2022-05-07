using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Flight.Data;
using Flight.Flights.Dtos;
using Flight.Flights.Exceptions;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flight.Flights.Features.GetAvailableFlights;

public class GetAvailableFlightsQueryHandler : IRequestHandler<GetAvailableFlightsQuery, IEnumerable<FlightResponseDto>>
{
    private readonly FlightDbContext _flightDbContext;
    private readonly IMapper _mapper;

    public GetAvailableFlightsQueryHandler(IMapper mapper, FlightDbContext flightDbContext)
    {
        _mapper = mapper;
        _flightDbContext = flightDbContext;
    }

    public async Task<IEnumerable<FlightResponseDto>> Handle(GetAvailableFlightsQuery query,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var flight = await _flightDbContext.Flights.Where(x => !x.IsDeleted).ToListAsync(cancellationToken);

        if (!flight.Any())
            throw new FlightNotFountException();

        return _mapper.Map<List<FlightResponseDto>>(flight);
    }
}
