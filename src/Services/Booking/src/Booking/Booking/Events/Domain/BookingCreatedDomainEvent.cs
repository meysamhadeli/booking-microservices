using Booking.Booking.Models.ValueObjects;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Core.Model;

namespace Booking.Booking.Events.Domain;

public record BookingCreatedDomainEvent(long Id, PassengerInfo PassengerInfo, Trip Trip) : Audit, IDomainEvent;
