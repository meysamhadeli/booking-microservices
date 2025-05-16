namespace Integration.Test.Fakes;
using BookingFlight;
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
