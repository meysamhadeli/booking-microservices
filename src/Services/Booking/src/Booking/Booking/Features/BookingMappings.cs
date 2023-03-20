using Booking.Booking.Dtos;
using Mapster;

namespace Booking.Booking.Features;

using CreatingBook.Commands.V1;

public class BookingMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);

        config.NewConfig<Models.Booking, BookingResponseDto>()
            .ConstructUsing(x => new BookingResponseDto(x.Id, x.PassengerInfo.Name, x.Trip.FlightNumber,
                x.Trip.AircraftId, x.Trip.Price, x.Trip.FlightDate, x.Trip.SeatNumber, x.Trip.DepartureAirportId, x.Trip.ArriveAirportId,
                x.Trip.Description));


        config.NewConfig<CreateBookingRequestDto, CreateBooking>()
            .ConstructUsing(x => new CreateBooking(x.PassengerId, x.FlightId, x.Description));
    }
}
