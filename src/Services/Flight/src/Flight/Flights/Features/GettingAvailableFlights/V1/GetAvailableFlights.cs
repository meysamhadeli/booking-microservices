namespace Flight.Flights.Features.GettingAvailableFlights.V1;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Caching;
using BuildingBlocks.Core.CQRS;
using Data;
using Dtos;
using Exceptions;
using MapsterMapper;
using MongoDB.Driver;

public record GetAvailableFlights : IQuery<GetAvailableFlightsResult>, ICacheRequest
{
    public string CacheKey => "GetAvailableFlights";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetAvailableFlightsResult(IEnumerable<FlightDto> FlightDtos);

internal class GetAvailableFlightsHandler : IQueryHandler<GetAvailableFlights, GetAvailableFlightsResult>
{
    private readonly IMapper _mapper;
    private readonly FlightReadDbContext _flightReadDbContext;

    public GetAvailableFlightsHandler(IMapper mapper, FlightReadDbContext flightReadDbContext)
    {
        _mapper = mapper;
        _flightReadDbContext = flightReadDbContext;
    }

    public async Task<GetAvailableFlightsResult> Handle(GetAvailableFlights request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var flight = (await _flightReadDbContext.Flight.AsQueryable().ToListAsync(cancellationToken))
            .Where(x => !x.IsDeleted);

        if (!flight.Any())
        {
            throw new FlightNotFountException();
        }

        var flightDtos = _mapper.Map<IEnumerable<FlightDto>>(flight);

        return new GetAvailableFlightsResult(flightDtos);
    }
}
