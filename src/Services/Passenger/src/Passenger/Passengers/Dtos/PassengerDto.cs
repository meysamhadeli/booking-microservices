namespace Passenger.Passengers.Dtos;

public record PassengerDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string PassportNumber { get; init; }
    public Enums.PassengerType PassengerType { get; init; }
    public int Age { get; init; }
}
