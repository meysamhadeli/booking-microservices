namespace Booking.Booking.Dtos;

public record BookingResponseDto
{
    public long Id { get; init; }
    public string Name { get; init; }
    public string FlightNumber { get; init; }
    public long AircraftId { get; init; }

    public decimal Price { get; init; }
    public DateTime FlightDate { get; init; }
    public string SeatNumber { get; init; }
    public long DepartureAirportId { get; init; }
    public long ArriveAirportId { get; init; }
    public string Description { get; init; }
}
