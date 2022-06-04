using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Contracts.Grpc;
using FluentAssertions;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcSamples;
using Integration.Test.Fakes;
using MagicOnion;
using MagicOnion.Client;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Integration.Test.Booking.Features;

[Collection(nameof(IntegrationTestFixture))]
public class CreateBookingTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly ITestHarness _testHarness;
    private readonly GrpcChannel _channel;
    private FooService.FooServiceClient _fooServiceClient;

    public CreateBookingTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _testHarness = fixture.TestHarness;
        _channel = fixture.Channel;
        _fooServiceClient = Substitute.For<FooService.FooServiceClient>();

        _fooServiceClient.GetFoo(Arg.Any<FooRequest>())
            .Returns(new FooResponse() {Message = "vvvvvvvvvvv"});

        fixture.RegisterTestServices(services =>
        {
            services.AddSingleton(_fooServiceClient);
        });
    }

    [Fact]
    public async Task should_create_booking_currectly()
    {
        // Arrange
        var command = new FakeCreateBookingCommand().Generate();

        // var passengerGrpcService = MagicOnionClient.Create<IPassengerGrpcService2>(_channel) as IPassengerGrpcService2;
        // var b = await passengerGrpcService.GetById(1);

        // var client = new FooService.FooServiceClient(_channel);
        //
        // var b = client.GetFoo(new FooRequest {Message = "tesssssssssst"});
        _fooServiceClient = Substitute.For<FooService.FooServiceClient>();

        _fooServiceClient.GetFooAsync(Arg.Any<FooRequest>())
            .Returns(new AsyncUnaryCall<FooResponse>(Task.FromResult(new FooResponse() {Message = "uuuuuuuuu"}), null, null, null, null));
        //


        // Act
        var response = await _fixture.SendAsync(command);

        // Assert
        response.Should();
    }


    // [Fact]
    // public async Task should_retrive_a_passenger_by_id_currectly()
    // {
    //     // Arrange
    //     var userCreated = new FakeUserCreated().Generate();
    //     await _testHarness.Bus.Publish(userCreated);
    //     await _testHarness.Consumed.Any<UserCreated>();
    //     var passengerEntity = FakePassengerCreated.Generate(userCreated);
    //     await _fixture.InsertAsync(passengerEntity);
    //
    //     var query = new GetPassengerQueryById(passengerEntity.Id);
    //
    //     // Act
    //     var response = await _fixture.SendAsync(query);
    //
    //     // Assert
    //     response.Should().NotBeNull();
    //     response?.Id.Should().Be(passengerEntity.Id);
    // }
    //
    // [Fact]
    // public async Task should_retrive_a_passenger_by_id_from_grpc_service()
    // {
    //     // Arrange
    //     var userCreated = new FakeUserCreated().Generate();
    //     await _testHarness.Bus.Publish(userCreated);
    //     await _testHarness.Consumed.Any<UserCreated>();
    //     var passengerEntity = FakePassengerCreated.Generate(userCreated);
    //     await _fixture.InsertAsync(passengerEntity);
    //
    //     var passengerGrpcClient = MagicOnionClient.Create<IPassengerGrpcService>(_channel);
    //
    //     // Act
    //     var response = await passengerGrpcClient.GetById(1111111111);
    //
    //     // Assert
    //     response?.Should().NotBeNull();
    //     response?.Id.Should().Be(passengerEntity.Id);
    // }
}
