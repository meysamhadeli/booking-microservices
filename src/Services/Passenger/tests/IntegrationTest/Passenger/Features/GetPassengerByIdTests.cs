using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Contracts.Grpc;
using BuildingBlocks.IdsGenerator;
using FluentAssertions;
using Grpc.Core;
using Grpc.Net.Client;
using Integration.Test.Fakes;
using MagicOnion;
using MagicOnion.Client;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NSubstitute;
using Passenger.Passengers.Features.GetPassengerById;
using Server;
using Test;
using Xunit;

namespace Integration.Test.Passenger.Features;

[Collection(nameof(IntegrationTestFixture))]
public class GetPassengerByIdTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly GrpcChannel _channel;
    private readonly ITestHarness _testHarness;
    private IPassengerGrpcService _passengerGrpcService;

    public GetPassengerByIdTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        var mockGreeter = Substitute.For<IGreeter>();

        mockGreeter.Greet(Arg.Any<string>())
            .Returns("heyyyyyyyyyyyyyyy");


        _fixture.RegisterTestServices(services =>
        {
            services.AddSingleton(mockGreeter);
        });

        _testHarness = _fixture.TestHarness;
        _channel = _fixture.Channel;
    }

    [Fact]
    public async Task should_retrive_a_passenger_by_id_currectly()
    {
        // Arrange
        var userCreated = new FakeUserCreated().Generate();
        await _testHarness.Bus.Publish(userCreated);
        await _testHarness.Consumed.Any<UserCreated>();
        var passengerEntity = FakePassengerCreated.Generate(userCreated);
        await _fixture.InsertAsync(passengerEntity);

        var query = new GetPassengerQueryById(passengerEntity.Id);

        // Act
        var response = await _fixture.SendAsync(query);

        // Assert
        response.Should().NotBeNull();
        response?.Id.Should().Be(passengerEntity.Id);
    }


    [Fact]
    public async Task should_retrive_a_passenger_by_id_from_grpc_service()
    {
        // Arrange
        var userCreated = new FakeUserCreated().Generate();
        await _testHarness.Bus.Publish(userCreated);
        await _testHarness.Consumed.Any<UserCreated>();
        var passengerEntity = FakePassengerCreated.Generate(userCreated);
        await _fixture.InsertAsync(passengerEntity);

        var passengerGrpcClient = MagicOnionClient.Create<IPassengerGrpcService>(_channel);

        // Act
        var response = await passengerGrpcClient.GetById(passengerEntity.Id);

        // Assert
        response?.Should().NotBeNull();
        response?.Id.Should().Be(passengerEntity.Id);
    }

    [Fact]
    public async Task should_retrive_a_passenger_by_id_from_grpc_service_2()
    {
        // Arrange
         var client = new Tester.TesterClient(_fixture.Channel);

         // Act
         var response = await client.SayHelloUnaryAsync(
             new HelloRequest {Name = "Joe"});

         // Assert
         Assert.Equal("heyyyyyyyyyyyyyyy", response.Message);
    }
}
