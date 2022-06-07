using Ardalis.GuardClauses;
using Booking.Booking.Exceptions;
using Booking.Booking.Models.ValueObjects;
using BuildingBlocks.Contracts.Grpc;
using BuildingBlocks.EventStoreDB.Repository;
using MediatR;

namespace Booking.Booking.Features.CreateBooking;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, ulong>
{
    private readonly IEventStoreDBRepository<Models.Booking> _eventStoreDbRepository;
    private readonly IFlightGrpcService _flightGrpcService;
    private readonly IPassengerGrpcService _passengerGrpcService;

    public CreateBookingCommandHandler(IEventStoreDBRepository<Models.Booking> eventStoreDbRepository,
        IPassengerGrpcService passengerGrpcService,
        IFlightGrpcService flightGrpcService)
    {
        _eventStoreDbRepository = eventStoreDbRepository;
        _passengerGrpcService = passengerGrpcService;
        _flightGrpcService = flightGrpcService;
    }

    public async Task<ulong> Handle(CreateBookingCommand command,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var flight = await _flightGrpcService.GetById(command.FlightId);

        if (flight is null)
            throw new FlightNotFoundException();

        var passenger = await _passengerGrpcService.GetById(command.PassengerId);

        var emptySeat = (await _flightGrpcService.GetAvailableSeats(command.FlightId))?.First();

        var reservation = await _eventStoreDbRepository.Find(command.Id, cancellationToken);

        if (reservation is not null && !reservation.IsDeleted)
            throw new BookingAlreadyExistException();

        var aggrigate = Models.Booking.Create(command.Id, new PassengerInfo(passenger.Name), new Trip(
            flight.FlightNumber, flight.AircraftId, flight.DepartureAirportId,
            flight.ArriveAirportId, flight.FlightDate, flight.Price, command.Description, emptySeat?.SeatNumber));

        await _flightGrpcService.ReserveSeat(new ReserveSeatRequestDto
        {
            FlightId = flight.Id, SeatNumber = emptySeat?.SeatNumber
        });

        var result = await _eventStoreDbRepository.Add(
            aggrigate,
            cancellationToken);

        return result;
    }
}
