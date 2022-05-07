using BuildingBlocks.Mongo;
using Flight.Flights.Models.Reads;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Flight.Data;

public class FlightReadDbContext : MongoDbContext
{
    public FlightReadDbContext(IOptions<MongoOptions> options) : base(options.Value)
    {
        Flight = GetCollection<FlightReadModel>(nameof(Flight).Underscore());
    }

    public IMongoCollection<FlightReadModel> Flight { get; }
}
