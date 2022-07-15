using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using Flight.Airports.Exceptions;
using Flight.Airports.Models.Reads;
using Flight.Data;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Flight.Airports.Features.CreateAirport.Reads;

public class CreateAirportMongoCommandHandler : ICommandHandler<CreateAirportMongoCommand>
{
    private readonly FlightReadDbContext _flightReadDbContext;
    private readonly IMapper _mapper;

    public CreateAirportMongoCommandHandler(
        FlightReadDbContext flightReadDbContext,
        IMapper mapper)
    {
        _flightReadDbContext = flightReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CreateAirportMongoCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var airportReadModel = _mapper.Map<AirportReadModel>(command);

        var aircraft = await _flightReadDbContext.Airport.AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == airportReadModel.Id, cancellationToken);

        if (aircraft is not null)
            throw new AirportAlreadyExistException();

        await _flightReadDbContext.Airport.InsertOneAsync(airportReadModel, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
