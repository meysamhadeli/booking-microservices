using Booking.Configuration;
using BuildingBlocks.Web;
using Flight;
using Microsoft.Extensions.DependencyInjection;
using Passenger;

namespace Booking.Extensions.Infrastructure;

public static class GrpcClientExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection services)
    {
        var grpcOptions = services.GetOptions<GrpcOptions>("Grpc");

        services.AddGrpcClient<FlightGrpcService.FlightGrpcServiceClient>(o =>
        {
            o.Address = new Uri(grpcOptions.FlightAddress);
        });

        services.AddGrpcClient<PassengerGrpcService.PassengerGrpcServiceClient>(o =>
        {
            o.Address = new Uri(grpcOptions.PassengerAddress);
        });

        return services;
    }
}
