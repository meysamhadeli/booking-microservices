using Booking.Api;
using Booking.Data;
using BuildingBlocks.EFCore;
using BuildingBlocks.PersistMessageProcessor.Data;
using BuildingBlocks.TestBase;
using Xunit;

namespace Integration.Test;

[Collection(IntegrationTestCollection.Name)]
public class BookingIntegrationTestBase: TestBase<Program, AppDbContextBase, BookingReadDbContext>
{
    public BookingIntegrationTestBase(TestFactory<Program, AppDbContextBase, BookingReadDbContext> integrationTestFixture) : base(integrationTestFixture)
    {
    }
}

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<TestFactory<Program, AppDbContextBase, BookingReadDbContext>>
{
    public const string Name = "Booking Integration Test";
}
