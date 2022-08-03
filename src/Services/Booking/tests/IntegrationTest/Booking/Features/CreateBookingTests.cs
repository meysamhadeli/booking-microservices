using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Booking.Booking.Models.Reads;
using Booking.Data;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Contracts.Grpc;
using BuildingBlocks.PersistMessageProcessor.Data;
using BuildingBlocks.TestBase;
using FluentAssertions;
using Integration.Test.Fakes;
using MagicOnion;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Xunit;

namespace Integration.Test.Booking.Features;

public class CreateBookingTests : IntegrationTestBase<Program, PersistMessageDbContext, BookingReadDbContext>
{
    private readonly ITestHarness _testHarness;
    public CreateBookingTests(
        IntegrationTestFixture<Program, PersistMessageDbContext, BookingReadDbContext> integrationTestFixture) : base(
        integrationTestFixture)
    {
        _testHarness = Fixture.TestHarness;
    }

    protected override void RegisterTestsServices(IServiceCollection services)
    {
        MockFlightGrpcServices(services);
        MockPassengerGrpcServices(services);
    }

    // todo: add support test for event-store
    [Fact]
    public async Task should_create_booking_to_event_store_currectly()
    {
        // Arrange
        var command = new FakeCreateBookingCommand().Generate();

        // Act
        var response = await Fixture.SendAsync(command);

        // Assert
        response.Should().BeGreaterOrEqualTo(0);
        (await _testHarness.Published.Any<Fault<BookingCreated>>()).Should().BeFalse();
        (await _testHarness.Published.Any<BookingCreated>()).Should().BeTrue();
    }


    private void MockPassengerGrpcServices(IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Singleton(x =>
        {
            var mock = Substitute.For<IPassengerGrpcService>();
            mock.GetById(Arg.Any<long>())
                .Returns(new UnaryResult<PassengerResponseDto>(new FakePassengerResponseDto().Generate()));

            return mock;
        }));
    }

    private void MockFlightGrpcServices(IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Singleton(x =>
        {
            var mock = Substitute.For<IFlightGrpcService>();

            mock.GetById(Arg.Any<long>())
                .Returns(new UnaryResult<FlightResponseDto>(Task.FromResult(new FakeFlightResponseDto().Generate())));

            mock.GetAvailableSeats(Arg.Any<long>())
                .Returns(
                    new UnaryResult<IEnumerable<SeatResponseDto>>(Task.FromResult(FakeSeatsResponseDto.Generate())));

            mock.ReserveSeat(new FakeReserveSeatRequestDto().Generate())
                .Returns(new UnaryResult<SeatResponseDto>(Task.FromResult(FakeSeatsResponseDto.Generate().First())));

            return mock;
        }));
    }
}
