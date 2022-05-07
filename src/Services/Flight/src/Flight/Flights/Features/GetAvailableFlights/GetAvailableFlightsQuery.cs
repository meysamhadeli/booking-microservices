using System;
using System.Collections.Generic;
using BuildingBlocks.Caching;
using Flight.Flights.Dtos;
using MediatR;

namespace Flight.Flights.Features.GetAvailableFlights;

public record GetAvailableFlightsQuery : IRequest<IEnumerable<FlightResponseDto>>, ICacheRequest
{
    public string CacheKey => "GetAvailableFlightsQuery";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}
