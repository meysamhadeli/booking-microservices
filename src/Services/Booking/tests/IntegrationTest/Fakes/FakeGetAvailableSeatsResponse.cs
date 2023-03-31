using System.Collections.Generic;
using Flight;

namespace Integration.Test.Fakes;

using System;
using MassTransit;

public static class FakeGetAvailableSeatsResponse
{
    public static GetAvailableSeatsResult Generate()
    {
        var result = new GetAvailableSeatsResult();
        result.SeatDtos.AddRange(new List<SeatDtoResponse>
        {
            new SeatDtoResponse()
            {
                FlightId = new Guid("3c5c0000-97c6-fc34-2eb9-08db322230c9").ToString(),
                Class = SeatClass.Economy,
                Type = SeatType.Aisle,
                SeatNumber = "33F",
                Id = NewId.NextGuid().ToString()
            },
            new SeatDtoResponse()
            {
                FlightId = new Guid("3c5c0000-97c6-fc34-2eb9-08db322230c9").ToString(),
                Class = SeatClass.Economy,
                Type = SeatType.Window,
                SeatNumber = "22D",
                Id = NewId.NextGuid().ToString()
            }
        });

        return result;
    }
}
