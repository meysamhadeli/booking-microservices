using System.Collections.Generic;
using Flight;

namespace Integration.Test.Fakes;

public static class FakeSeatsResponse
{
    public static ListSeatsResponse Generate()
    {
        var result = new ListSeatsResponse();
        result.Items.AddRange(new List<SeatsResponse>
        {
            new SeatsResponse()
            {
                FlightId = 1,
                Class = SeatClass.Economy,
                Type = SeatType.Aisle,
                SeatNumber = "33F",
                Id = 1
            },
            new SeatsResponse()
            {
                FlightId = 1,
                Class = SeatClass.Economy,
                Type = SeatType.Window,
                SeatNumber = "22D",
                Id = 2
            }
        });

        return result;
    }
}
