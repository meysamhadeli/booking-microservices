namespace Booking.Booking.Features.CreatingBook.Commands.V1;

using BuildingBlocks.Core.Event;
using BuildingBlocks.Core.Model;
using Models.ValueObjects;

public record BookingCreatedDomainEvent(Guid Id, PassengerInfo PassengerInfo, Trip Trip) : Audit, IDomainEvent;
