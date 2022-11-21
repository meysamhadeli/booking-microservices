using Booking.Booking.Dtos;
using Booking.Booking.Features.CreateBooking.Commands.V1;
using Booking.Booking.Features.CreateBooking.Dtos.V1;
using Mapster;

namespace Booking.Booking.Features;

public class BookingMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);

        config.NewConfig<Models.Booking, BookingResponseDto>()
            .ConstructUsing(x => new BookingResponseDto(x.Id, x.PassengerInfo.Name, x.Trip.FlightNumber,
                x.Trip.AircraftId, x.Trip.Price, x.Trip.FlightDate, x.Trip.SeatNumber, x.Trip.DepartureAirportId, x.Trip.ArriveAirportId,
                x.Trip.Description));


        config.NewConfig<CreateBookingRequestDto, CreateBookingCommand>()
            .ConstructUsing(x => new CreateBookingCommand(x.PassengerId, x.FlightId, x.Description));
    }
}
