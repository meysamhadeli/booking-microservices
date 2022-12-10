using BuildingBlocks.TestBase;
using Identity.Api;
using Identity.Data;
using Xunit;

namespace Integration.Test;

[Collection(IntegrationTestCollection.Name)]
public class IdentityIntegrationTestBase: TestBase<Program, IdentityContext>
{
    public IdentityIntegrationTestBase(TestFactory<Program, IdentityContext> integrationTestFactory)
        : base(integrationTestFactory)
    {
    }
}

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<TestFactory<Program, IdentityContext>>
{
    public const string Name = "Identity Integration Test";
}
