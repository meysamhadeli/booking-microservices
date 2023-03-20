namespace Flight.Seats.Features.GettingAvailableSeats.V1;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using Flight.Data;
using Flight.Seats.Dtos;
using Flight.Seats.Exceptions;
using FluentValidation;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;

public record GetAvailableSeats(long FlightId) : IQuery<IEnumerable<SeatDto>>;

internal class GetAvailableSeatsValidator : AbstractValidator<GetAvailableSeats>
{
    public GetAvailableSeatsValidator()
    {
        RuleFor(x => x.FlightId).NotNull().WithMessage("FlightId is required!");
    }
}

internal class GetAvailableSeatsQueryHandler : IRequestHandler<GetAvailableSeats, IEnumerable<SeatDto>>
{
    private readonly IMapper _mapper;
    private readonly FlightReadDbContext _flightReadDbContext;

    public GetAvailableSeatsQueryHandler(IMapper mapper, FlightReadDbContext flightReadDbContext)
    {
        _mapper = mapper;
        _flightReadDbContext = flightReadDbContext;
    }


    public async Task<IEnumerable<SeatDto>> Handle(GetAvailableSeats query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var seats = (await _flightReadDbContext.Seat.AsQueryable().ToListAsync(cancellationToken))
            .Where(x => x.FlightId == query.FlightId && !x.IsDeleted);

        if (!seats.Any())
        {
            throw new AllSeatsFullException();
        }

        return _mapper.Map<IEnumerable<SeatDto>>(seats);
    }
}
