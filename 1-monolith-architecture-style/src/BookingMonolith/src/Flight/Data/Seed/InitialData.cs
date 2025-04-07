using BookingMonolith.Flight.Aircrafts.Models;
using BookingMonolith.Flight.Aircrafts.ValueObjects;
using BookingMonolith.Flight.Airports.Models;
using BookingMonolith.Flight.Airports.ValueObjects;
using BookingMonolith.Flight.Flights.ValueObjects;
using BookingMonolith.Flight.Seats.Models;
using BookingMonolith.Flight.Seats.ValueObjects;
using MassTransit;

namespace BookingMonolith.Flight.Data.Seed;

using AirportName = Airports.ValueObjects.Name;
using Name = Aircrafts.ValueObjects.Name;

public static class InitialData
{
    public static List<Airport> Airports { get; }
    public static List<Aircraft> Aircrafts { get; }
    public static List<Seat> Seats { get; }
    public static List<Flights.Models.Flight> Flights { get; }


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


        Flights = new List<Flights.Models.Flight>
        {
            Flight.Flights.Models.Flight.Create(FlightId.Of(new Guid("3c5c0000-97c6-fc34-2eb9-08db322230c9")), FlightNumber.Of("BD467"), AircraftId.Of(Aircrafts.First().Id.Value), AirportId.Of( Airports.First().Id), DepartureDate.Of(new DateTime(2022, 1, 31, 12, 0, 0)),
               ArriveDate.Of(new DateTime(2022, 1, 31, 14, 0, 0)),
               AirportId.Of(Airports.Last().Id), DurationMinutes.Of(120m),
                FlightDate.Of(new DateTime(2022, 1, 31, 13, 0, 0)), Flight.Flights.Enums.FlightStatus.Completed,
                Price.Of(8000))
        };

        Seats = new List<Seat>
        {
            Seat.Create(SeatId.Of(NewId.NextGuid()), SeatNumber.Of( "12A"),Flight.Seats.Enums.SeatType.Window, Flight.Seats.Enums.SeatClass.Economy, FlightId.Of((Guid)Flights.First().Id)),
            Seat.Create(SeatId.Of(NewId.NextGuid()), SeatNumber.Of("12B"), Flight.Seats.Enums.SeatType.Window, Flight.Seats.Enums.SeatClass.Economy, FlightId.Of((Guid)Flights.First().Id)),
            Seat.Create(SeatId.Of(NewId.NextGuid()), SeatNumber.Of("12C"), Flight.Seats.Enums.SeatType.Middle, Flight.Seats.Enums.SeatClass.Economy, FlightId.Of((Guid) Flights.First().Id)),
            Seat.Create(SeatId.Of(NewId.NextGuid()), SeatNumber.Of("12D"), Flight.Seats.Enums.SeatType.Middle, Flight.Seats.Enums.SeatClass.Economy, FlightId.Of((Guid) Flights.First().Id)),
            Seat.Create(SeatId.Of(NewId.NextGuid()), SeatNumber.Of("12E"), Flight.Seats.Enums.SeatType.Aisle, Flight.Seats.Enums.SeatClass.Economy, FlightId.Of((Guid) Flights.First().Id)),
            Seat.Create(SeatId.Of(NewId.NextGuid()), SeatNumber.Of("12F"), Flight.Seats.Enums.SeatType.Aisle, Flight.Seats.Enums.SeatClass.Economy, FlightId.Of((Guid) Flights.First().Id))
        };
    }
}
