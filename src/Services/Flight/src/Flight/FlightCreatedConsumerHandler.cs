using System;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using MassTransit;

namespace Flight;

public class FlightCreatedConsumerHandler : IConsumer<FlightCreated>
{
    public Task Consume(ConsumeContext<FlightCreated> context)
    {
        Console.WriteLine($"We retrieve message: {context.Message}");
        return Task.CompletedTask;
    }
}
