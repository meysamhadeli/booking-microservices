using System;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using MassTransit;

namespace Flight;

public class FlightCreatedConsumerHandler : IConsumer<FlightCreated>
{
    public Task Consume(ConsumeContext<FlightCreated> context)
    {
        Console.WriteLine("It's for test");
        return Task.CompletedTask;
    }
}
