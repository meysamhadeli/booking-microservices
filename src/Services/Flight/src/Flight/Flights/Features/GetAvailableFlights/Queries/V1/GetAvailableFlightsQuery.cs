using System;
using System.Collections.Generic;
using BuildingBlocks.Caching;
using BuildingBlocks.Core.CQRS;
using Flight.Flights.Dtos;

namespace Flight.Flights.Features.GetAvailableFlights.Queries.V1;

public record GetAvailableFlightsQuery : IQuery<IEnumerable<FlightResponseDto>>, ICacheRequest
{
    public string CacheKey => "GetAvailableFlightsQuery";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}
