namespace Passenger.Passengers.Features.CompletingRegisterPassenger.V1;

using BuildingBlocks.Web;
using Hellang.Middleware.ProblemDetails;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Dtos;
using Swashbuckle.AspNetCore.Annotations;

public record CompleteRegisterPassengerRequestDto(string PassportNumber, Enums.PassengerType PassengerType, int Age);
public record CompleteRegisterPassengerResponseDto(PassengerDto PassengerDto);

public class CompleteRegisterPassengerEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/passenger/complete-registration", CompleteRegisterPassenger)
            .RequireAuthorization()
            .WithTags("Passenger")
            .WithName("CompleteRegisterPassenger")
            .WithMetadata(new SwaggerOperationAttribute("Complete Register Passenger", "Complete Register Passenger"))
            .WithApiVersionSet(builder.NewApiVersionSet("Passenger").Build())
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status200OK,
                    "Register Passenger Completed",
                    typeof(CompleteRegisterPassengerResponseDto)))
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status400BadRequest,
                    "BadRequest",
                    typeof(StatusCodeProblemDetails)))
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status401Unauthorized,
                    "UnAuthorized",
                    typeof(StatusCodeProblemDetails)))
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
