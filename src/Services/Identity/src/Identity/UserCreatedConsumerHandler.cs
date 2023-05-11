namespace Identity;

using System;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using MassTransit;

public class UserCreatedConsumerHandler : IConsumer<UserCreated>
{
    public Task Consume(ConsumeContext<UserCreated> context)
    {
        Console.WriteLine($"We retrieve message: {context.Message}");
        return Task.CompletedTask;
    }
}
