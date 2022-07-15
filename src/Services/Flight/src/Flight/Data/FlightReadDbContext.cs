using BuildingBlocks.Mongo;
using Flight.Aircrafts.Models.Reads;
using Flight.Airports.Models;
using Flight.Airports.Models.Reads;
using Flight.Flights.Models.Reads;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Flight.Data;

public class FlightReadDbContext : MongoDbContext
{
    public FlightReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        Flight = GetCollection<FlightReadModel>(nameof(Flight).Underscore());
        Aircraft = GetCollection<AircraftReadModel>(nameof(Aircraft).Underscore());
        Airport = GetCollection<AirportReadModel>(nameof(Airport).Underscore());
    }

    public IMongoCollection<FlightReadModel> Flight { get; }
    public IMongoCollection<AircraftReadModel> Aircraft { get; }
    public IMongoCollection<AirportReadModel> Airport { get; }
}
