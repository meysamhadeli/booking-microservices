namespace Passenger.Passengers.Dtos;
public record PassengerDto(Guid Id, string Name, string PassportNumber, Enums.PassengerType PassengerType, int Age);
