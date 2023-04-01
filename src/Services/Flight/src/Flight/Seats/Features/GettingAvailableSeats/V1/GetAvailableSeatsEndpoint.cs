namespace Flight.Seats.Features.GettingAvailableSeats.V1;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Dtos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public record GetAvailableSeatsResponseDto(IEnumerable<SeatDto> SeatDtos);

public class GetAvailableSeatsEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/flight/get-available-seats/{{id}}", GetAvailableSeats)
            .RequireAuthorization()
            .WithName("GetAvailableSeats")
            .WithMetadata(new SwaggerOperationAttribute("Get Available Seats", "Get Available Seats"))
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<GetAvailableSeatsResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> GetAvailableSeats(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAvailableSeats(id), cancellationToken);

        var response = new GetAvailableSeatsResponseDto(result?.SeatDtos);

        return Results.Ok(response);
    }
}
