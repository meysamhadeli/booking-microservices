using System;
using System.Threading.Tasks;
using Xunit;

namespace Integration.Test;

[Collection(nameof(TestFixture))]
public class DeleteTests
{
    private readonly TestFixture _fixture;

    public DeleteTests(TestFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_delete_flight()
    {
        var b = 2;
    }

}
