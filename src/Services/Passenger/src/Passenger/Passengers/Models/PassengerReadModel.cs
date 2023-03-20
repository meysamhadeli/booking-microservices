namespace Passenger.Passengers.Models;

public class PassengerReadModel
{
    public long Id { get; init; }
    public long PassengerId { get; init; }
    public string PassportNumber { get; init; }
    public string Name { get; init; }
    public Enums.PassengerType PassengerType { get; init; }
    public int Age { get; init; }
    public bool IsDeleted { get; init; }
}
