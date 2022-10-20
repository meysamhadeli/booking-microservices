using Booking.Booking.Models.ValueObjects;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Core.Model;

namespace Booking.Booking.Features.CreateBooking.Events.Domain.V1;

public record BookingCreatedDomainEvent(long Id, PassengerInfo PassengerInfo, Trip Trip) : Audit, IDomainEvent;
