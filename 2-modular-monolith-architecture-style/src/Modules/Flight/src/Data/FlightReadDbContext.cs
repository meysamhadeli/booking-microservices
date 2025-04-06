using BuildingBlocks.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Flight.Data;

using Aircrafts.Models;
using Airports.Models;
using Flights.Models;
using Seats.Models;

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
