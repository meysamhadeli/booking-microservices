namespace Flight.Airports.Features.CreatingAirport.V1;

using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using BuildingBlocks.IdsGenerator;
using Dtos;
using Exceptions;
using Data;
using FluentValidation;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record CreateAirport(string Name, string Address, string Code) : ICommand<AirportDto>, IInternalCommand
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}

internal class CreateAirportValidator : AbstractValidator<CreateAirport>
{
    public CreateAirportValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("Code is required");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required");
    }
}


internal class CreateAirportHandler : IRequestHandler<CreateAirport, AirportDto>
{
    private readonly FlightDbContext _flightDbContext;
    private readonly IMapper _mapper;

    public CreateAirportHandler(IMapper mapper, FlightDbContext flightDbContext)
    {
        _mapper = mapper;
        _flightDbContext = flightDbContext;
    }

    public async Task<AirportDto> Handle(CreateAirport request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var airport = await _flightDbContext.Airports.SingleOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

        if (airport is not null)
        {
            throw new AirportAlreadyExistException();
        }

        var airportEntity = Models.Airport.Create(request.Id, request.Name, request.Code, request.Address);

        var newAirport = await _flightDbContext.Airports.AddAsync(airportEntity, cancellationToken);

        await _flightDbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AirportDto>(newAirport.Entity);
    }
}
