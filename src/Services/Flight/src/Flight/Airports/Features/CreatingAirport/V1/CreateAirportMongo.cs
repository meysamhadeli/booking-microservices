namespace Flight.Airports.Features.CreatingAirport.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Exceptions;
using Models;
using Data;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

public record CreateAirportMongo(Guid Id, string Name, string Address, string Code, bool IsDeleted) : InternalCommand;

internal class CreateAirportMongoHandler : ICommandHandler<CreateAirportMongo>
{
    private readonly FlightReadDbContext _flightReadDbContext;
    private readonly IMapper _mapper;

    public CreateAirportMongoHandler(
        FlightReadDbContext flightReadDbContext,
        IMapper mapper)
    {
        _flightReadDbContext = flightReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CreateAirportMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var airportReadModel = _mapper.Map<AirportReadModel>(request);

        var aircraft = await _flightReadDbContext.Airport.AsQueryable()
            .FirstOrDefaultAsync(x => x.AirportId == airportReadModel.AirportId, cancellationToken);

        if (aircraft is not null)
        {
            throw new AirportAlreadyExistException();
        }

        await _flightReadDbContext.Airport.InsertOneAsync(airportReadModel, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
