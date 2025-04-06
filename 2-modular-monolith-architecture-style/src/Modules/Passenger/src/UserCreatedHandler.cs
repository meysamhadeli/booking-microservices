// using BuildingBlocks.Contracts.EventBus.Messages;
// using MassTransit;
//
// namespace Passenger;
//
// public class UserCreatedHandler : IConsumer<UserCreated>
// {
//     public Task Consume(ConsumeContext<UserCreated> context)
//     {
//         Console.WriteLine(context.Message.PassportNumber);
//         return Task.CompletedTask;
//     }
// }
