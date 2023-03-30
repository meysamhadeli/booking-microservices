namespace Passenger.Passengers.Features.CompletingRegisterPassenger.V1;

using BuildingBlocks.Web;
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
            .WithName("CompleteRegisterPassenger")
            .WithMetadata(new SwaggerOperationAttribute("Complete Register Passenger", "Complete Register Passenger"))
            .WithApiVersionSet(builder.NewApiVersionSet("Passenger").Build())
            .Produces<CompleteRegisterPassengerResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
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
