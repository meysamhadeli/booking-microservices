using MagicOnion;
using MessagePack;

namespace BuildingBlocks.Contracts.Grpc;


    public interface IFlightGrpcService : IService<IFlightGrpcService>
    {
        UnaryResult<FlightResponseDto> GetById(long id);
        UnaryResult<IEnumerable<SeatResponseDto>> GetAvailableSeats(long flightId);
        UnaryResult<FlightResponseDto> ReserveSeat(ReserveSeatRequestDto request);
    }


    [MessagePackObject]
    public class ReserveSeatRequestDto
    {
        [Key(0)]
        public long FlightId { get; set; }
        [Key(1)]
        public string SeatNumber { get; set; }
    }

    [MessagePackObject]
    public record SeatResponseDto
    {
        [Key(0)]
        public long Id { get; set; }
        [Key(1)]
        public string SeatNumber { get; init; }
        [Key(2)]
        public SeatType Type { get; init; }
        [Key(3)]
        public SeatClass Class { get; init; }
        [Key(4)]
        public long FlightId { get; init; }
    }

    [MessagePackObject]
    public record FlightResponseDto
    {
        [Key(0)]
        public long Id { get; init; }
        [Key(1)]
        public string FlightNumber { get; init; }
        [Key(2)]
        public long AircraftId { get; init; }
        [Key(3)]
        public long DepartureAirportId { get; init; }
        [Key(4)]
        public DateTime DepartureDate { get; init; }
        [Key(5)]
        public DateTime ArriveDate { get; init; }
        [Key(6)]
        public long ArriveAirportId { get; init; }
        [Key(7)]
        public decimal DurationMinutes { get; init; }
        [Key(8)]
        public DateTime FlightDate { get; init; }
        [Key(9)]
        public FlightStatus Status { get; init; }
        [Key(10)]
        public decimal Price { get; init; }
    }

    public enum FlightStatus
    {
        Flying = 1,
        Delay = 2,
        Canceled = 3,
        Completed = 4
    }

    public enum SeatType
    {
        Window,
        Middle,
        Aisle
    }

    public enum SeatClass
    {
        FirstClass,
        Business,
        Economy
    }

