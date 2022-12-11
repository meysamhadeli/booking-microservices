using BuildingBlocks.TestBase;
using Passenger.Api;
using Passenger.Data;
using Xunit;

namespace Integration.Test;

[Collection(IntegrationTestCollection.Name)]
public class PassengerIntegrationTestBase: TestWriteBase<Program, PassengerDbContext>
{
    public PassengerIntegrationTestBase(TestWriteFixture<Program, PassengerDbContext> integrationTestFactory)
        : base(integrationTestFactory)
    {
    }
}

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<TestWriteFixture<Program, PassengerDbContext>>
{
    public const string Name = "Passenger Integration Test";
}
