using Booking.Data;
using Griffin.TestBase;
using Xunit;

namespace Integration.Test;

[Collection(IntegrationTestCollection.Name)]
public class BookingIntegrationTestBase : TestReadBase<Program, BookingReadDbContext>
{
    public BookingIntegrationTestBase(TestReadFixture<Program, BookingReadDbContext> integrationTestFixture)
        : base(integrationTestFixture) { }
}

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<TestReadFixture<Program, BookingReadDbContext>>
{
    public const string Name = "Booking Integration Test";
}
