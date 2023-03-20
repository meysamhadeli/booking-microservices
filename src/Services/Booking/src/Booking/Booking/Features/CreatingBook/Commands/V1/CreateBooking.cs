namespace Booking.Booking.Features.CreatingBook.Commands.V1;

using Ardalis.GuardClauses;
using BuildingBlocks.Core;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using BuildingBlocks.EventStoreDB.Repository;
using BuildingBlocks.IdsGenerator;
using BuildingBlocks.Web;
using Exceptions;
using Flight;
using FluentValidation;
using Models.ValueObjects;
using Passenger;

public record CreateBooking(long PassengerId, long FlightId, string Description) : ICommand<ulong>, IInternalCommand
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}

internal class CreateBookingValidator : AbstractValidator<CreateBooking>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.FlightId).NotNull().WithMessage("FlightId is required!");
        RuleFor(x => x.PassengerId).NotNull().WithMessage("PassengerId is required!");
    }
}

internal class CreateBookingCommandHandler : ICommandHandler<CreateBooking, ulong>
{
    private readonly IEventStoreDBRepository<Models.Booking> _eventStoreDbRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly FlightGrpcService.FlightGrpcServiceClient _flightGrpcServiceClient;
    private readonly PassengerGrpcService.PassengerGrpcServiceClient _passengerGrpcServiceClient;

    public CreateBookingCommandHandler(IEventStoreDBRepository<Models.Booking> eventStoreDbRepository,
        ICurrentUserProvider currentUserProvider,
        IEventDispatcher eventDispatcher,
        FlightGrpcService.FlightGrpcServiceClient flightGrpcServiceClient,
        PassengerGrpcService.PassengerGrpcServiceClient passengerGrpcServiceClient)
    {
        _eventStoreDbRepository = eventStoreDbRepository;
        _currentUserProvider = currentUserProvider;
        _eventDispatcher = eventDispatcher;
        _flightGrpcServiceClient = flightGrpcServiceClient;
        _passengerGrpcServiceClient = passengerGrpcServiceClient;
    }

    public async Task<ulong> Handle(CreateBooking command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var flight = await _flightGrpcServiceClient.GetByIdAsync(new Flight.GetByIdRequest { Id = command.FlightId });

        if (flight is null)
        {
            throw new FlightNotFoundException();
        }

        var passenger =
            await _passengerGrpcServiceClient.GetByIdAsync(new Passenger.GetByIdRequest { Id = command.PassengerId });

        var emptySeat = (await _flightGrpcServiceClient
                .GetAvailableSeatsAsync(new GetAvailableSeatsRequest { FlightId = command.FlightId }).ResponseAsync)
            ?.Items?.FirstOrDefault();

        var reservation = await _eventStoreDbRepository.Find(command.Id, cancellationToken);

        if (reservation is not null && !reservation.IsDeleted)
        {
            throw new BookingAlreadyExistException();
        }

        var aggrigate = Models.Booking.Create(command.Id, new PassengerInfo(passenger.Name), new Trip(
                flight.FlightNumber, flight.AircraftId, flight.DepartureAirportId,
                flight.ArriveAirportId, flight.FlightDate.ToDateTime(), (decimal)flight.Price, command.Description,
                emptySeat?.SeatNumber),
            false, _currentUserProvider.GetCurrentUserId());

        await _eventDispatcher.SendAsync(aggrigate.DomainEvents, cancellationToken: cancellationToken);

        await _flightGrpcServiceClient.ReserveSeatAsync(new ReserveSeatRequest
        {
            FlightId = flight.Id, SeatNumber = emptySeat?.SeatNumber
        });

        var result = await _eventStoreDbRepository.Add(
            aggrigate,
            cancellationToken);

        return result;
    }
}
