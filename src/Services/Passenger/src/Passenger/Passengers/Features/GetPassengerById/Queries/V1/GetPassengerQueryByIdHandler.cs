using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Passenger.Data;
using Passenger.Passengers.Dtos;
using Passenger.Passengers.Features.GetPassengerById.Exceptions;

namespace Passenger.Passengers.Features.GetPassengerById.Queries.V1;

public class GetPassengerQueryByIdHandler : IQueryHandler<GetPassengerQueryById, PassengerResponseDto>
{
    private readonly PassengerDbContext _passengerDbContext;
    private readonly IMapper _mapper;

    public GetPassengerQueryByIdHandler(IMapper mapper, PassengerDbContext passengerDbContext)
    {
        _mapper = mapper;
        _passengerDbContext = passengerDbContext;
    }

    public async Task<PassengerResponseDto> Handle(GetPassengerQueryById query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var passenger =
            await _passengerDbContext.Passengers.SingleOrDefaultAsync(x => x.Id == query.Id, cancellationToken);

        if (passenger is null)
            throw new PassengerNotFoundException();

        return _mapper.Map<PassengerResponseDto>(passenger!);
    }
}
