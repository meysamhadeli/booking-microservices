using BuildingBlocks.TestBase;
using Passenger.Api;
using Passenger.Data;
using Xunit;

namespace Integration.Test;

[Collection(IntegrationTestCollection.Name)]
public class PassengerIntegrationTestBase: TestBase<Program, PassengerDbContext>
{
    public PassengerIntegrationTestBase(TestFactory<Program, PassengerDbContext> integrationTestFactory)
        : base(integrationTestFactory)
    {
    }
}

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<TestFactory<Program, PassengerDbContext>>
{
    public const string Name = "Passenger Integration Test";
}
