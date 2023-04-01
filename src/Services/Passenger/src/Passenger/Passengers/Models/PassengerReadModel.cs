namespace Passenger.Passengers.Models;

public class PassengerReadModel
{
    public Guid Id { get; init; }
    public Guid PassengerId { get; init; }
    public string PassportNumber { get; init; }
    public string Name { get; init; }
    public Enums.PassengerType PassengerType { get; init; }
    public int Age { get; init; }
    public bool IsDeleted { get; init; }
}
