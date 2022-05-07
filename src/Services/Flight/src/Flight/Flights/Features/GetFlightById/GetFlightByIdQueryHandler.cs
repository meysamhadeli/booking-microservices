using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Flight.Data;
using Flight.Flights.Dtos;
using Flight.Flights.Exceptions;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flight.Flights.Features.GetFlightById;

public class GetFlightByIdQueryHandler : IRequestHandler<GetFlightByIdQuery, FlightResponseDto>
{
    private readonly FlightDbContext _flightDbContext;
    private readonly IMapper _mapper;

    public GetFlightByIdQueryHandler(IMapper mapper, FlightDbContext flightDbContext)
    {
        _mapper = mapper;
        _flightDbContext = flightDbContext;
    }

    public async Task<FlightResponseDto> Handle(GetFlightByIdQuery query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var flight =
            await _flightDbContext.Flights.SingleOrDefaultAsync(x => x.Id == query.Id && !x.IsDeleted, cancellationToken);

        if (flight is null)
            throw new FlightNotFountException();

        return _mapper.Map<FlightResponseDto>(flight);
    }
}
