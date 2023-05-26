namespace Passenger.Passengers.Features.GettingPassengerById.Queries.V1;

using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Web;
using Data;
using Dtos;
using Duende.IdentityServer.EntityFramework.Entities;
using Exceptions;
using FluentValidation;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Models.ValueObjects;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

public record GetPassengerById(Guid Id) : IQuery<GetPassengerByIdResult>;

public record GetPassengerByIdResult(PassengerDto PassengerDto);

public record GetPassengerByIdResponseDto(PassengerDto PassengerDto);

public class GetPassengerByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/passenger/{{id}}",
                async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetPassengerById(id), cancellationToken);

                    var response = new GetPassengerByIdResponseDto(result?.PassengerDto);

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetPassengerById")
            .WithApiVersionSet(builder.NewApiVersionSet("Passenger").Build())
            .Produces<GetPassengerByIdResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Passenger By Id")
            .WithDescription("Get Passenger By Id")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetPassengerByIdValidator : AbstractValidator<GetPassengerById>
{
    public GetPassengerByIdValidator()
    {
        RuleFor(x => x.Id).NotNull().WithMessage("Id is required!");
    }
}

internal class GetPassengerByIdHandler : IQueryHandler<GetPassengerById, GetPassengerByIdResult>
{
    private readonly IMapper _mapper;
    private readonly PassengerReadDbContext _passengerReadDbContext;

    public GetPassengerByIdHandler(IMapper mapper, PassengerReadDbContext passengerReadDbContext)
    {
        _mapper = mapper;
        _passengerReadDbContext = passengerReadDbContext;
    }

    public async Task<GetPassengerByIdResult> Handle(GetPassengerById query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var passenger =
            await _passengerReadDbContext.Passenger.AsQueryable()
                .SingleOrDefaultAsync(x => x.PassengerId == PassengerId.Of(query.Id) && x.IsDeleted == false, cancellationToken);

        if (passenger is null)
        {
            throw new PassengerNotFoundException();
        }

        var passengerDto = _mapper.Map<PassengerDto>(passenger);

        return new GetPassengerByIdResult(passengerDto);
    }
}
