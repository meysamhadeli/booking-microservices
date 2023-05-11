namespace Identity;

using System;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using MassTransit;

public class UserCreatedConsumerHandler : IConsumer<UserCreated>
{
    public Task Consume(ConsumeContext<UserCreated> context)
    {
        Console.WriteLine("It's for test"); return Task.CompletedTask;
    }
}
