namespace Passenger.Passengers.Features.CompletingRegisterPassenger.V1;

using BuildingBlocks.Web;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Dtos;
using Microsoft.OpenApi.Models;

public record CompleteRegisterPassengerRequestDto(string PassportNumber, Enums.PassengerType PassengerType, int Age);
public record CompleteRegisterPassengerResponseDto(PassengerDto PassengerDto);

public class CompleteRegisterPassengerEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/passenger/complete-registration", CompleteRegisterPassenger)
            .RequireAuthorization()
            .WithName("CompleteRegisterPassenger")
            .WithApiVersionSet(builder.NewApiVersionSet("Passenger").Build())
            .Produces<CompleteRegisterPassengerResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation => new OpenApiOperation(operation) { Summary = "Complete Register Passenger", Description = "Complete Register Passenger" })
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> CompleteRegisterPassenger(CompleteRegisterPassengerRequestDto request, IMapper mapper,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var command = mapper.Map<CompleteRegisterPassenger>(request);

        var result = await mediator.Send(command, cancellationToken);

        var response = new CompleteRegisterPassengerResponseDto(result?.PassengerDto);

        return Results.Ok(response);
    }
}
