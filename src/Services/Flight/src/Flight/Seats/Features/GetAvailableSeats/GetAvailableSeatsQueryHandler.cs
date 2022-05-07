using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Flight.Data;
using Flight.Seats.Dtos;
using Flight.Seats.Exceptions;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flight.Seats.Features.GetAvailableSeats;

public class GetAvailableSeatsQueryHandler : IRequestHandler<GetAvailableSeatsQuery, IEnumerable<SeatResponseDto>>
{
    private readonly FlightDbContext _flightDbContext;
    private readonly IMapper _mapper;

    public GetAvailableSeatsQueryHandler(IMapper mapper, FlightDbContext flightDbContext)
    {
        _mapper = mapper;
        _flightDbContext = flightDbContext;
    }


    public async Task<IEnumerable<SeatResponseDto>> Handle(GetAvailableSeatsQuery query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var seats = await _flightDbContext.Seats.Where(x => x.FlightId == query.FlightId && !x.IsDeleted).ToListAsync(cancellationToken);

        if (!seats.Any())
            throw new AllSeatsFullException();

        return _mapper.Map<List<SeatResponseDto>>(seats);
    }
}
