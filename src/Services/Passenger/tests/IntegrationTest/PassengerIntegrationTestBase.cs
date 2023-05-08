using BuildingBlocks.TestBase;
using Passenger.Api;
using Passenger.Data;
using Xunit;

namespace Integration.Test;

[Collection(IntegrationTestCollection.Name)]
public class PassengerIntegrationTestBase: TestBase<Program, PassengerDbContext, PassengerReadDbContext>
{
    public PassengerIntegrationTestBase(TestFixture<Program, PassengerDbContext, PassengerReadDbContext> integrationTestFactory)
        : base(integrationTestFactory)
    {
    }
}

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<TestFixture<Program, PassengerDbContext, PassengerReadDbContext>>
{
    public const string Name = "Passenger Integration Test";
}
