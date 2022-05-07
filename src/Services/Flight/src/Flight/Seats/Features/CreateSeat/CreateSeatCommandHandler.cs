using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.EventStoreDB.Repository;
using Flight.Airports.Exceptions;
using Flight.Airports.Features.CreateAirport;
using Flight.Airports.Models;
using Flight.Data;
using Flight.Seats.Dtos;
using Flight.Seats.Exceptions;
using Flight.Seats.Models;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flight.Seats.Features.CreateSeat;

public class CreateSeatCommandHandler : IRequestHandler<CreateSeatCommand, SeatResponseDto>
{
    private readonly FlightDbContext _flightDbContext;
    private readonly IMapper _mapper;

    public CreateSeatCommandHandler(IMapper mapper, FlightDbContext flightDbContext)
    {
        _mapper = mapper;
        _flightDbContext = flightDbContext;
    }

    public async Task<SeatResponseDto> Handle(CreateSeatCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var seat = await _flightDbContext.Seats.SingleOrDefaultAsync(x => x.Id == command.Id && !x.IsDeleted, cancellationToken);

        if (seat is not null)
            throw new SeatAlreadyExistException();

        var seatEntity = Seat.Create(command.Id, command.SeatNumber, command.Type, command.Class, command.FlightId);

        var newSeat = await _flightDbContext.Seats.AddAsync(seatEntity, cancellationToken);

        return _mapper.Map<SeatResponseDto>(newSeat.Entity);
    }
}
