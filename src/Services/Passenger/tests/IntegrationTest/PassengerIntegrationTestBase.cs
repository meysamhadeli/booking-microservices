using BuildingBlocks.TestBase.IntegrationTest;
using Passenger.Api;
using Passenger.Data;
using Xunit;

namespace Integration.Test;

[Collection(IntegrationTestCollection.Name)]
public class PassengerIntegrationTestBase: IntegrationTestBase<Program, PassengerDbContext>
{
    public PassengerIntegrationTestBase(IntegrationTestFactory<Program, PassengerDbContext> integrationTestFactory)
        : base(integrationTestFactory)
    {
    }
}

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFactory<Program, PassengerDbContext>>
{
    public const string Name = "Passenger Integration Test";
}
