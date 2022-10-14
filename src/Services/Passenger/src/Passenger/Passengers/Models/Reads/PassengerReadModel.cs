namespace Passenger.Passengers.Models.Reads;

public class PassengerReadModel
{
    public long Id { get; init; }
    public long PassengerId { get; init; }
    public string PassportNumber { get; private set; }
    public string Name { get; private set; }
    public Enums.PassengerType PassengerType { get; private set; }
    public int Age { get; private set; }
    public bool IsDeleted { get; init; }
}
