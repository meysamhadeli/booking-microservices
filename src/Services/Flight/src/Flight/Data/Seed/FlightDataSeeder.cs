using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.EFCore;
using Flight.Aircrafts.Models;
using Flight.Aircrafts.Models.Reads;
using Flight.Airports.Models;
using Flight.Airports.Models.Reads;
using Flight.Flights.Models;
using Flight.Flights.Models.Reads;
using Flight.Seats.Models;
using Flight.Seats.Models.Reads;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Flight.Data.Seed;

public class FlightDataSeeder : IDataSeeder
{
    private readonly FlightDbContext _flightDbContext;
    private readonly FlightReadDbContext _flightReadDbContext;
    private readonly IMapper _mapper;

    public FlightDataSeeder(FlightDbContext flightDbContext,
        FlightReadDbContext flightReadDbContext,
        IMapper mapper)
    {
        _flightDbContext = flightDbContext;
        _flightReadDbContext = flightReadDbContext;
        _mapper = mapper;
    }

    public async Task SeedAllAsync()
    {
        await SeedAirportAsync();
        await SeedAircraftAsync();
        await SeedFlightAsync();
        await SeedSeatAsync();
    }

    private async Task SeedAirportAsync()
    {
        if (!await _flightDbContext.Airports.AnyAsync())
        {
            var airports = new List<Airport>
            {
                Airport.Create(1, "Lisbon International Airport", "LIS", "12988"),
                Airport.Create(2, "Sao Paulo International Airport", "BRZ", "11200")
            };

            await _flightDbContext.Airports.AddRangeAsync(airports);
            await _flightDbContext.SaveChangesAsync();
            await _flightReadDbContext.Airport.InsertManyAsync(_mapper.Map<List<AirportReadModel>>(airports));
        }
    }

    private async Task SeedAircraftAsync()
    {
        if (!await _flightDbContext.Aircraft.AnyAsync())
        {
            var aircrafts = new List<Aircraft>
            {
                Aircraft.Create(1, "Boeing 737", "B737", 2005),
                Aircraft.Create(2, "Airbus 300", "A300", 2000),
                Aircraft.Create(3, "Airbus 320", "A320", 2003)
            };

            await _flightDbContext.Aircraft.AddRangeAsync(aircrafts);
            await _flightDbContext.SaveChangesAsync();
            await _flightReadDbContext.Aircraft.InsertManyAsync(_mapper.Map<List<AircraftReadModel>>(aircrafts));
        }
    }


    private async Task SeedSeatAsync()
    {
        if (!await _flightDbContext.Seats.AnyAsync())
        {
            var seats = new List<Seat>
            {
                Seat.Create(1 ,"12A", SeatType.Window, SeatClass.Economy, 1),
                Seat.Create(2, "12B", SeatType.Window, SeatClass.Economy, 1),
                Seat.Create(3, "12C", SeatType.Middle, SeatClass.Economy, 1),
                Seat.Create(4, "12D", SeatType.Middle, SeatClass.Economy, 1),
                Seat.Create(5, "12E", SeatType.Aisle, SeatClass.Economy, 1),
                Seat.Create(6, "12F", SeatType.Aisle, SeatClass.Economy, 1)
            };

            await _flightDbContext.Seats.AddRangeAsync(seats);
            await _flightDbContext.SaveChangesAsync();
            await _flightReadDbContext.Seat.InsertManyAsync(_mapper.Map<List<SeatReadModel>>(seats));
        }
    }

    private async Task SeedFlightAsync()
    {
        if (!await _flightDbContext.Flights.AnyAsync())
        {
            var flights = new List<Flights.Models.Flight>
            {
                Flights.Models.Flight.Create(1, "BD467", 1, 1, new DateTime(2022, 1, 31, 12, 0, 0),
                    new DateTime(2022, 1, 31, 14, 0, 0),
                    2, 120m,
                    new DateTime(2022, 1, 31), FlightStatus.Completed,
                    8000)
            };
            await _flightDbContext.Flights.AddRangeAsync(flights);
            await _flightDbContext.SaveChangesAsync();
            await _flightReadDbContext.Flight.InsertManyAsync(_mapper.Map<List<FlightReadModel>>(flights));
        }
    }
}
