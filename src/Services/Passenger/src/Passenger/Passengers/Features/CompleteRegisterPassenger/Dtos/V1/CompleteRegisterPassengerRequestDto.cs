namespace Passenger.Passengers.Features.CompleteRegisterPassenger.Dtos.V1;

public record CompleteRegisterPassengerRequestDto(string PassportNumber, Enums.PassengerType PassengerType, int Age);
