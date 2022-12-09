using Ardalis.GuardClauses;
using Booking.Booking.Features.CreateBooking.Exceptions;
using Booking.Booking.Models.ValueObjects;
using BuildingBlocks.Core;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.EventStoreDB.Repository;
using BuildingBlocks.Web;
using Flight;
using Passenger;

namespace Booking.Booking.Features.CreateBooking.Commands.V1;

public class CreateBookingCommandHandler : ICommandHandler<CreateBookingCommand, ulong>
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

    public async Task<ulong> Handle(CreateBookingCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var flight = await _flightGrpcServiceClient.GetByIdAsync(new Flight.GetByIdRequest {Id = command.FlightId});

        if (flight is null)
            throw new FlightNotFoundException();

        var passenger =
            await _passengerGrpcServiceClient.GetByIdAsync(new Passenger.GetByIdRequest {Id = command.PassengerId});

        var emptySeat = (await _flightGrpcServiceClient
                .GetAvailableSeatsAsync(new GetAvailableSeatsRequest {FlightId = command.FlightId}).ResponseAsync)
            ?.Items?.FirstOrDefault();

        var reservation = await _eventStoreDbRepository.Find(command.Id, cancellationToken);

        if (reservation is not null && !reservation.IsDeleted)
            throw new BookingAlreadyExistException();

        var aggrigate = Models.Booking.Create(command.Id, new PassengerInfo(passenger.Name), new Trip(
                flight.FlightNumber, flight.AircraftId, flight.DepartureAirportId,
                flight.ArriveAirportId, flight.FlightDate.ToDateTime(), (decimal)flight.Price, command.Description,
                emptySeat?.SeatNumber),
            false, _currentUserProvider.GetCurrentUserId());

        await _eventDispatcher.SendAsync(aggrigate.DomainEvents, cancellationToken: cancellationToken);

        await _flightGrpcServiceClient.ReserveSeatAsync(new ReserveSeatRequest
        {
            FlightId = flight.FlightId, SeatNumber = emptySeat?.SeatNumber
        });

        var result = await _eventStoreDbRepository.Add(
            aggrigate,
            cancellationToken);

        return result;
    }
}
