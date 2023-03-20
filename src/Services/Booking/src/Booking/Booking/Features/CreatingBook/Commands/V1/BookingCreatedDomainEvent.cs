namespace Booking.Booking.Features.CreatingBook.Commands.V1;

using BuildingBlocks.Core.Event;
using BuildingBlocks.Core.Model;
using Models.ValueObjects;

public record BookingCreatedDomainEvent(long Id, PassengerInfo PassengerInfo, Trip Trip) : Audit, IDomainEvent;
