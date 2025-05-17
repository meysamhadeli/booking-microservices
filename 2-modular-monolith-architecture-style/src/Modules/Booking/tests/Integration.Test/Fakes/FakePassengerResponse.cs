namespace Integration.Test.Fakes;

using BookingPassenger;
using MassTransit;

public static class FakePassengerResponse
{
    public static GetPassengerByIdResult Generate()
    {
        var result = new GetPassengerByIdResult
        {
            PassengerDto = new PassengerResponse()
            {
                Id = NewId.NextGuid().ToString(),
                Name = "Test",
                PassportNumber = "121618"
            }
        };

        return result;
    }
}
