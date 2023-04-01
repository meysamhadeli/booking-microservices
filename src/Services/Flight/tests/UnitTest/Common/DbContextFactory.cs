using System;
using System.Collections.Generic;
using Flight.Data;
using Flight.Flights.Enums;
using Flight.Seats.Enums;
using Microsoft.EntityFrameworkCore;

namespace Unit.Test.Common
{
    using MassTransit;

    public static class DbContextFactory
    {
        private static readonly Guid _airportId1 = NewId.NextGuid();
        private static readonly Guid _airportId2 = NewId.NextGuid();
        private static readonly Guid _aircraft1 = NewId.NextGuid();
        private static readonly Guid _aircraft2 = NewId.NextGuid();
        private static readonly Guid _aircraft3 = NewId.NextGuid();
        private static readonly Guid _flightId1 = NewId.NextGuid();

        public static FlightDbContext Create()
        {
            var options = new DbContextOptionsBuilder<FlightDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            var context = new FlightDbContext(options, currentUserProvider: null);

            // Seed our data
            FlightDataSeeder(context);

            return context;
        }

        private static void FlightDataSeeder(FlightDbContext context)
        {
            var airports = new List<global::Flight.Airports.Models.Airport>
            {
                global::Flight.Airports.Models.Airport.Create(_airportId1, "Lisbon International Airport", "LIS", "12988"),
                global::Flight.Airports.Models.Airport.Create(_airportId2, "Sao Paulo International Airport", "BRZ", "11200")
            };

            context.Airports.AddRange(airports);

            var aircrafts = new List<global::Flight.Aircrafts.Models.Aircraft>
            {
                global::Flight.Aircrafts.Models.Aircraft.Create(_aircraft1, "Boeing 737", "B737", 2005),
                global::Flight.Aircrafts.Models.Aircraft.Create(_aircraft2, "Airbus 300", "A300", 2000),
                global::Flight.Aircrafts.Models.Aircraft.Create(_aircraft3, "Airbus 320", "A320", 2003)
            };

            context.Aircraft.AddRange(aircrafts);

            var flights = new List<global::Flight.Flights.Models.Flight>
            {
                global::Flight.Flights.Models.Flight.Create(_flightId1, "BD467", _aircraft1, _airportId1, new DateTime(2022, 1, 31, 12, 0, 0),
                    new DateTime(2022, 1, 31, 14, 0, 0),
                    _airportId2, 120m,
                    new DateTime(2022, 1, 31), FlightStatus.Completed,
                    8000)
            };
            context.Flights.AddRange(flights);

            var seats = new List<global::Flight.Seats.Models.Seat>
            {
                global::Flight.Seats.Models.Seat.Create(NewId.NextGuid(), "12A", SeatType.Window, SeatClass.Economy, _flightId1),
                global::Flight.Seats.Models.Seat.Create(NewId.NextGuid(), "12B", SeatType.Window, SeatClass.Economy, _flightId1),
                global::Flight.Seats.Models.Seat.Create(NewId.NextGuid(), "12C", SeatType.Middle, SeatClass.Economy, _flightId1),
                global::Flight.Seats.Models.Seat.Create(NewId.NextGuid(), "12D", SeatType.Middle, SeatClass.Economy, _flightId1),
                global::Flight.Seats.Models.Seat.Create(NewId.NextGuid(), "12E", SeatType.Aisle, SeatClass.Economy, _flightId1),
                global::Flight.Seats.Models.Seat.Create(NewId.NextGuid(), "12F", SeatType.Aisle, SeatClass.Economy, _flightId1)
            };

            context.Seats.AddRange(seats);

            context.SaveChanges();
        }

        public static void Destroy(FlightDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}
