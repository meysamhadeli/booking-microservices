using System.Linq;
using System.Threading.Tasks;
using Booking.Api;
using Booking.Data;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.PersistMessageProcessor.Data;
using BuildingBlocks.TestBase;
using Flight;
using FluentAssertions;
using Grpc.Core;
using Grpc.Core.Testing;
using Integration.Test.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Passenger;
using Xunit;
using GetByIdRequest = Flight.GetByIdRequest;

namespace Integration.Test.Booking.Features;

public class CreateBookingTests : BookingIntegrationTestBase
{
    public CreateBookingTests(IntegrationTestFactory<Program, PersistMessageDbContext, BookingReadDbContext> integrationTestFixture) : base(integrationTestFixture)
    {
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

        (await Fixture.WaitForPublishing<BookingCreated>()).Should().Be(true);
    }


    private void MockPassengerGrpcServices(IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Singleton(x =>
        {
            var mockPassenger = Substitute.For<PassengerGrpcService.PassengerGrpcServiceClient>();

            mockPassenger.GetByIdAsync(Arg.Any<Passenger.GetByIdRequest>())
                .Returns(TestCalls.AsyncUnaryCall(Task.FromResult(new FakePassengerResponse().Generate()),
                    Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { }));

            return mockPassenger;
        }));
    }

    private void MockFlightGrpcServices(IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Singleton(x =>
        {
            var mockFlight = Substitute.For<FlightGrpcService.FlightGrpcServiceClient>();

            mockFlight.GetByIdAsync(Arg.Any<GetByIdRequest>())
                .Returns(TestCalls.AsyncUnaryCall(Task.FromResult(new FakeFlightResponse().Generate()),
                    Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { }));

            mockFlight.GetAvailableSeatsAsync(Arg.Any<GetAvailableSeatsRequest>())
                .Returns(TestCalls.AsyncUnaryCall(Task.FromResult(FakeSeatsResponse.Generate()),
                    Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { }));

            mockFlight.ReserveSeatAsync(Arg.Any<ReserveSeatRequest>())
                .Returns(TestCalls.AsyncUnaryCall(Task.FromResult(FakeSeatsResponse.Generate()?.Items?.First()),
                    Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { }));

            return mockFlight;
        }));
    }
}
