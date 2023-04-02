namespace Passenger.Passengers.Models;

public class PassengerReadModel
{
    public required Guid Id { get; init; }
    public required Guid PassengerId { get; init; }
    public required string PassportNumber { get; init; }
    public required string Name { get; init; }
    public required Enums.PassengerType PassengerType { get; init; }
    public int Age { get; init; }
    public required bool IsDeleted { get; init; }
}
