using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingBlocks.EFCore;
using Flight.Aircrafts.Models;
using Flight.Airports.Models;
using Flight.Seats.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Flight.Data.Seed;

using Flights.Models;

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
            await _flightDbContext.Airports.AddRangeAsync(InitialData.Airports);
            await _flightDbContext.SaveChangesAsync();

            if (!await _flightReadDbContext.Airport.AsQueryable().AnyAsync())
            {
                await _flightReadDbContext.Airport.InsertManyAsync(_mapper.Map<List<AirportReadModel>>(InitialData.Airports));
            }
        }
    }

    private async Task SeedAircraftAsync()
    {
        if (!await _flightDbContext.Aircraft.AnyAsync())
        {
            await _flightDbContext.Aircraft.AddRangeAsync(InitialData.Aircrafts);
            await _flightDbContext.SaveChangesAsync();

            if (!await _flightReadDbContext.Aircraft.AsQueryable().AnyAsync())
            {
                await _flightReadDbContext.Aircraft.InsertManyAsync(_mapper.Map<List<AircraftReadModel>>(InitialData.Aircrafts));
            }
        }
    }


    private async Task SeedSeatAsync()
    {
        if (!await _flightDbContext.Seats.AnyAsync())
        {
            await _flightDbContext.Seats.AddRangeAsync(InitialData.Seats);
            await _flightDbContext.SaveChangesAsync();

            if (!await _flightReadDbContext.Seat.AsQueryable().AnyAsync())
            {
                await _flightReadDbContext.Seat.InsertManyAsync(_mapper.Map<List<SeatReadModel>>(InitialData.Seats));
            }
        }
    }

    private async Task SeedFlightAsync()
    {
        if (!await _flightDbContext.Flights.AnyAsync())
        {
            await _flightDbContext.Flights.AddRangeAsync(InitialData.Flights);
            await _flightDbContext.SaveChangesAsync();

            if (!await _flightReadDbContext.Flight.AsQueryable().AnyAsync())
            {
                await _flightReadDbContext.Flight.InsertManyAsync(_mapper.Map<List<FlightReadModel>>(InitialData.Flights));
            }
        }
    }
}
