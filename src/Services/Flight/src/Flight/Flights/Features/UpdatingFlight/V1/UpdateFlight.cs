namespace Flight.Flights.Features.UpdatingFlight.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Caching;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Flight.Data;
using Flight.Flights.Dtos;
using Flight.Flights.Exceptions;
using Flight.Flights.Features.CreatingFlight.V1;
using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

public record UpdateFlight(long Id, string FlightNumber, long AircraftId, long DepartureAirportId,
    DateTime DepartureDate, DateTime ArriveDate, long ArriveAirportId, decimal DurationMinutes, DateTime FlightDate,
    Enums.FlightStatus Status, bool IsDeleted, decimal Price) : ICommand<FlightDto>, IInternalCommand, IInvalidateCacheRequest
{
    public string CacheKey => "GetAvailableFlights";
}

internal class UpdateFlightValidator : AbstractValidator<CreateFlight>
{
    public UpdateFlightValidator()
    {
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Status).Must(p => (p.GetType().IsEnum &&
                                          p == Enums.FlightStatus.Flying) ||
                                         p == Enums.FlightStatus.Canceled ||
                                         p == Enums.FlightStatus.Delay ||
                                         p == Enums.FlightStatus.Completed)
            .WithMessage("Status must be Flying, Delay, Canceled or Completed");

        RuleFor(x => x.AircraftId).NotEmpty().WithMessage("AircraftId must be not empty");
        RuleFor(x => x.DepartureAirportId).NotEmpty().WithMessage("DepartureAirportId must be not empty");
        RuleFor(x => x.ArriveAirportId).NotEmpty().WithMessage("ArriveAirportId must be not empty");
        RuleFor(x => x.DurationMinutes).GreaterThan(0).WithMessage("DurationMinutes must be greater than 0");
        RuleFor(x => x.FlightDate).NotEmpty().WithMessage("FlightDate must be not empty");
    }
}

internal class UpdateFlightHandler : ICommandHandler<UpdateFlight, FlightDto>
{
    private readonly FlightDbContext _flightDbContext;
    private readonly IMapper _mapper;

    public UpdateFlightHandler(IMapper mapper, FlightDbContext flightDbContext)
    {
        _mapper = mapper;
        _flightDbContext = flightDbContext;
    }

    public async Task<FlightDto> Handle(UpdateFlight request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var flight = await _flightDbContext.Flights.SingleOrDefaultAsync(x => x.Id == request.Id,
            cancellationToken);

        if (flight is null)
        {
            throw new FlightNotFountException();
        }


        flight.Update(request.Id, request.FlightNumber, request.AircraftId, request.DepartureAirportId, request.DepartureDate,
            request.ArriveDate, request.ArriveAirportId, request.DurationMinutes, request.FlightDate, request.Status, request.Price, request.IsDeleted);

        var updateFlight = _flightDbContext.Flights.Update(flight);

        return _mapper.Map<FlightDto>(updateFlight.Entity);
    }
}
