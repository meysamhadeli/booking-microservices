using Booking.Configuration;
using BuildingBlocks.Contracts.Grpc;
using BuildingBlocks.Web;
using Grpc.Net.Client;
using MagicOnion.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Extensions;

public static class MagicOnionClientExtensions
{
    public static IServiceCollection AddMagicOnionClients(this IServiceCollection services)
    {
        var grpcOptions = services.GetOptions<GrpcOptions>("Grpc");

        services.AddSingleton(x => MagicOnionClient.Create<IPassengerGrpcService>(GrpcChannel.ForAddress(grpcOptions.PassengerAddress)));
        services.AddSingleton(x => MagicOnionClient.Create<IFlightGrpcService>(GrpcChannel.ForAddress(grpcOptions.FlightAddress)));

        return services;
    }
}
