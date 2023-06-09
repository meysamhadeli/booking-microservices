namespace Flight.Data.Seed;

using System;
using System.Collections.Generic;
using System.Linq;
using Aircrafts.Models;
using Airports.Models;
using Airports.ValueObjects;
using Flight.Aircrafts.ValueObjects;
using Flights.Models;
using Flights.ValueObjects;
using MassTransit;
using Seats.Models;
using Seats.ValueObjects;
using AirportName = Airports.ValueObjects.Name;
using Name = Aircrafts.ValueObjects.Name;

public static class InitialData
{
    public static List<Airport> Airports { get; }
    public static List<Aircraft> Aircrafts { get; }
    public static List<Seat> Seats { get; }
    public static List<Flight> Flights { get; }


    static InitialData()
    {
        Airports = new List<Airport>
        {
            Airport.Create(AirportId.Of(new Guid("3c5c0000-97c6-fc34-a0cb-08db322230c8")), AirportName.Of("Lisbon International Airport"), Address.Of("LIS"), Code.Of("12988")),
            Airport.Create(AirportId.Of(new Guid("3c5c0000-97c6-fc34-fc3c-08db322230c8")), AirportName.Of("Sao Paulo International Airport"), Address.Of("BRZ"), Code.Of("11200"))
        };

        Aircrafts = new List<Aircraft>
        {
            Aircraft.Create(AircraftId.Of(new Guid("3c5c0000-97c6-fc34-fcd3-08db322230c8")), Name.Of("Boeing 737"), Model.Of("B737"), ManufacturingYear.Of(2005)),
            Aircraft.Create(AircraftId.Of(new Guid("3c5c0000-97c6-fc34-2e04-08db322230c9")), Name.Of("Airbus 300"), Model.Of("A300"), ManufacturingYear.Of(2000)),
            Aircraft.Create(AircraftId.Of(new Guid("3c5c0000-97c6-fc34-2e11-08db322230c9")), Name.Of("Airbus 320"), Model.Of("A320"), ManufacturingYear.Of(2003))
        };


        Flights = new List<Flight>
        {
            Flight.Create(FlightId.Of(new Guid("3c5c0000-97c6-fc34-2eb9-08db322230c9")), FlightNumber.Of("BD467"), AircraftId.Of(Aircrafts.First().Id.Value), AirportId.Of( Airports.First().Id), DepartureDate.Of(new DateTime(2022, 1, 31, 12, 0, 0)),
               ArriveDate.Of(new DateTime(2022, 1, 31, 14, 0, 0)),
               AirportId.Of(Airports.Last().Id), DurationMinutes.Of(120m),
                FlightDate.Of(new DateTime(2022, 1, 31, 13, 0, 0)), global::Flight.Flights.Enums.FlightStatus.Completed,
                Price.Of((decimal)8000))
        };

        Seats = new List<Seat>
        {
            Seat.Create(SeatId.Of(NewId.NextGuid()), SeatNumber.Of( "12A"), global::Flight.Seats.Enums.SeatType.Window, global::Flight.Seats.Enums.SeatClass.Economy, FlightId.Of((Guid)Flights.First().Id)),
            Seat.Create(SeatId.Of(NewId.NextGuid()), SeatNumber.Of("12B"), global::Flight.Seats.Enums.SeatType.Window, global::Flight.Seats.Enums.SeatClass.Economy, FlightId.Of((Guid)Flights.First().Id)),
            Seat.Create(SeatId.Of(NewId.NextGuid()), SeatNumber.Of("12C"), global::Flight.Seats.Enums.SeatType.Middle, global::Flight.Seats.Enums.SeatClass.Economy, FlightId.Of((Guid) Flights.First().Id)),
            Seat.Create(SeatId.Of(NewId.NextGuid()), SeatNumber.Of("12D"), global::Flight.Seats.Enums.SeatType.Middle, global::Flight.Seats.Enums.SeatClass.Economy, FlightId.Of((Guid) Flights.First().Id)),
            Seat.Create(SeatId.Of(NewId.NextGuid()), SeatNumber.Of("12E"), global::Flight.Seats.Enums.SeatType.Aisle, global::Flight.Seats.Enums.SeatClass.Economy, FlightId.Of((Guid) Flights.First().Id)),
            Seat.Create(SeatId.Of(NewId.NextGuid()), SeatNumber.Of("12F"), global::Flight.Seats.Enums.SeatType.Aisle, global::Flight.Seats.Enums.SeatClass.Economy, FlightId.Of((Guid) Flights.First().Id))
        };
    }
}
