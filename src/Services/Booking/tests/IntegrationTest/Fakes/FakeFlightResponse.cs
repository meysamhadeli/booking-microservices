using System;
using AutoBogus;
using Flight;
using Google.Protobuf.WellKnownTypes;

namespace Integration.Test.Fakes;

public static class FakeFlightResponse
{
    public static GetFlightByIdResult Generate()
    {
        var flightMock = new GetFlightByIdResult
        {
            FlightDto = new FlightResponse
            {
                Id = new Guid("3c5c0000-97c6-fc34-2eb9-08db322230c9").ToString(),
                Price = 100,
                Status = FlightStatus.Completed,
                AircraftId = new Guid("3c5c0000-97c6-fc34-fcd3-08db322230c8").ToString(),
                ArriveAirportId = new Guid("3c5c0000-97c6-fc34-a0cb-08db322230c8").ToString(),
                ArriveDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc).ToTimestamp(),
                DepartureDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc).ToTimestamp(),
                FlightDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc).ToTimestamp(),
                FlightNumber = "1500B",
                DepartureAirportId = new Guid("3c5c0000-97c6-fc34-fc3c-08db322230c8").ToString()
            }
        };

        return flightMock;
    }
}
