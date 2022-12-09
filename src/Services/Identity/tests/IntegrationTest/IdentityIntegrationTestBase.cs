using BuildingBlocks.TestBase;
using Identity.Api;
using Identity.Data;
using Xunit;

namespace Integration.Test;

[Collection(IntegrationTestCollection.Name)]
public class IdentityIntegrationTestBase: IntegrationTestBase<Program, IdentityContext>
{
    public IdentityIntegrationTestBase(IntegrationTestFactory<Program, IdentityContext> integrationTestFactory)
        : base(integrationTestFactory)
    {
    }
}

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFactory<Program, IdentityContext>>
{
    public const string Name = "Identity Integration Test";
}
