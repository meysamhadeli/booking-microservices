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
using FluentValidation;
using MapsterMapper;
using MongoDB.Driver;

public record GetAvailableFlights : IQuery<IEnumerable<FlightDto>>, ICacheRequest
{
    public string CacheKey => "GetAvailableFlights";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

internal class GetAvailableFlightsValidator : AbstractValidator<GetAvailableFlights>
{
}

internal class GetAvailableFlightsHandler : IQueryHandler<GetAvailableFlights, IEnumerable<FlightDto>>
{
    private readonly IMapper _mapper;
    private readonly FlightReadDbContext _flightReadDbContext;

    public GetAvailableFlightsHandler(IMapper mapper, FlightReadDbContext flightReadDbContext)
    {
        _mapper = mapper;
        _flightReadDbContext = flightReadDbContext;
    }

    public async Task<IEnumerable<FlightDto>> Handle(GetAvailableFlights request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var flight = (await _flightReadDbContext.Flight.AsQueryable().ToListAsync(cancellationToken))
            .Where(x => !x.IsDeleted);

        if (!flight.Any())
        {
            throw new FlightNotFountException();
        }

        return _mapper.Map<IEnumerable<FlightDto>>(flight);
    }
}
