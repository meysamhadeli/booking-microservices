using System;
using System.Threading;
using System.Threading.Tasks;
using Flight.Flights.Dtos;
using Flight.Flights.Features.CreateFlight;
using FluentAssertions;
using Unit.Test.Common;
using Unit.Test.Fakes;
using Xunit;

namespace Unit.Test.Flight.Features.CreateFlight;

[Collection(nameof(UnitTestFixture))]
public class CreateFlightCommandHandlerTests
{
    private readonly UnitTestFixture _fixture;
    private readonly CreateFlightCommandHandler _handler;

    public Task<FlightResponseDto> Act(CreateFlightCommand command, CancellationToken cancellationToken) =>
        _handler.Handle(command, cancellationToken);

    public CreateFlightCommandHandlerTests(UnitTestFixture fixture)
    {
        _fixture = fixture;
        _handler = new CreateFlightCommandHandler(fixture.Mapper, fixture.DbContext);
    }

    [Fact]
    public async Task handler_with_valid_command_should_create_new_flight_and_return_currect_flight_dto()
    {
        // Arrange
        var command = new FakeCreateFlightCommand().Generate();

        // Act
        var response = await Act(command, CancellationToken.None);

        // Assert
        var entity = await _fixture.DbContext.Flights.FindAsync(response?.Id);

        entity?.Should().NotBeNull();
        response?.Id.Should().Be(entity?.Id);
        response?.FlightNumber.Should().Be(entity?.FlightNumber);
    }

    [Fact]
    public async Task handler_with_null_command_should_throw_argument_exception()
    {
        // Arrange
        CreateFlightCommand command = null;

        // Act
        Func<Task> act = async () => { await Act(command, CancellationToken.None); };

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
