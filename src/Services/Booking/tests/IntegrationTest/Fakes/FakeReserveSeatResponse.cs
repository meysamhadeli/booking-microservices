namespace Integration.Test.Fakes;
using Flight;
using MassTransit;

public static class FakeReserveSeatResponse
{
    public static ReserveSeatResult Generate()
    {
        var result = new ReserveSeatResult();
        result.Id = NewId.NextGuid().ToString();

        return result;
    }
}
