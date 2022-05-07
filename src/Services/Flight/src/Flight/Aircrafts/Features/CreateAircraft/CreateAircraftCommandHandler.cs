using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Flight.Aircrafts.Dtos;
using Flight.Aircrafts.Exceptions;
using Flight.Aircrafts.Models;
using Flight.Data;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flight.Aircrafts.Features.CreateAircraft;

public class CreateAircraftCommandHandler : IRequestHandler<CreateAircraftCommand, AircraftResponseDto>
{
    private readonly FlightDbContext _flightDbContext;
    private readonly IMapper _mapper;

    public CreateAircraftCommandHandler(IMapper mapper, FlightDbContext flightDbContext)
    {
        _mapper = mapper;
        _flightDbContext = flightDbContext;
    }

    public async Task<AircraftResponseDto> Handle(CreateAircraftCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var aircraft = await _flightDbContext.Aircraft.SingleOrDefaultAsync(x => x.Model == command.Model, cancellationToken);

        if (aircraft is not null)
            throw new AircraftAlreadyExistException();

        var aircraftEntity = Aircraft.Create(command.Id, command.Name, command.Model, command.ManufacturingYear);

        var newAircraft = await _flightDbContext.Aircraft.AddAsync(aircraftEntity, cancellationToken);

        await _flightDbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AircraftResponseDto>(newAircraft.Entity);
    }
}
