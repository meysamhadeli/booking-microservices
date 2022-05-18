using System;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using MassTransit;

namespace Flight;

public class FlightConsumer : IConsumer<FlightCreated>
{
    public Task Consume(ConsumeContext<FlightCreated> context)
    {
        Console.WriteLine("This consumer is for test");
        return Task.CompletedTask;
    }
}
