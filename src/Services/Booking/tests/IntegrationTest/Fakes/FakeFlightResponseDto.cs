using System;
using AutoBogus;
using BuildingBlocks.Contracts.Grpc;

namespace Integration.Test.Fakes;

public class FakeFlightResponseDto : AutoFaker<FlightResponseDto>
{
    public FakeFlightResponseDto()
    {
        RuleFor(r => r.Id, _ => 1);
        RuleFor(r => r.Price, _ => 100);
        RuleFor(r => r.Status, _ => FlightStatus.Completed);
        RuleFor(r => r.AircraftId, _ => 1);
        RuleFor(r => r.ArriveAirportId, _ => 1);
        RuleFor(r => r.ArriveDate, _ => DateTime.Now);
        RuleFor(r => r.DepartureDate, _ => DateTime.Now);
        RuleFor(r => r.FlightDate, _ => DateTime.Now);
        RuleFor(r => r.FlightNumber, _ => "121LP");
        RuleFor(r => r.DepartureAirportId, _ => 2);
    }
}
