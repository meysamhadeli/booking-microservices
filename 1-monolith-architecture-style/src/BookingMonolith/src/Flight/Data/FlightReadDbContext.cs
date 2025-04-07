using BookingMonolith.Flight.Aircrafts.Models;
using BookingMonolith.Flight.Airports.Models;
using BookingMonolith.Flight.Flights.Models;
using BookingMonolith.Flight.Seats.Models;
using BuildingBlocks.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookingMonolith.Flight.Data;

public class FlightReadDbContext : MongoDbContext
{
    public FlightReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        Flight = GetCollection<FlightReadModel>(nameof(Flight).Underscore());
        Aircraft = GetCollection<AircraftReadModel>(nameof(Aircraft).Underscore());
        Airport = GetCollection<AirportReadModel>(nameof(Airport).Underscore());
        Seat = GetCollection<SeatReadModel>(nameof(Seat).Underscore());
    }

    public IMongoCollection<FlightReadModel> Flight { get; }
    public IMongoCollection<AircraftReadModel> Aircraft { get; }
    public IMongoCollection<AirportReadModel> Airport { get; }
    public IMongoCollection<SeatReadModel> Seat { get; }
}
