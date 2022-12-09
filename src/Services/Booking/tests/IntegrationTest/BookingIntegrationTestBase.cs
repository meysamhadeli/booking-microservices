using Booking.Api;
using Booking.Data;
using BuildingBlocks.PersistMessageProcessor.Data;
using BuildingBlocks.TestBase.IntegrationTest;
using Xunit;

namespace Integration.Test;

[Collection(IntegrationTestCollection.Name)]
public class BookingIntegrationTestBase: IntegrationTestBase<Program, PersistMessageDbContext, BookingReadDbContext>
{
    public BookingIntegrationTestBase(IntegrationTestFactory<Program, PersistMessageDbContext, BookingReadDbContext> integrationTestFixture) : base(integrationTestFixture)
    {
    }
}

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFactory<Program, PersistMessageDbContext, BookingReadDbContext>>
{
    public const string Name = "Booking Integration Test";
}
