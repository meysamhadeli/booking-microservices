using BuildingBlocks.TestBase;
using Flight.Api;
using Flight.Data;
using Xunit;

namespace Integration.Test;

[Collection(IntegrationTestCollection.Name)]
public class FlightIntegrationTestBase: IntegrationTestBase<Program, FlightDbContext, FlightReadDbContext>
{
    public FlightIntegrationTestBase(IntegrationTestFactory<Program, FlightDbContext, FlightReadDbContext> integrationTestFixture) : base(integrationTestFixture)
    {
    }
}

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFactory<Program, FlightDbContext, FlightReadDbContext>>
{
    public const string Name = "Flight Integration Test";
}
