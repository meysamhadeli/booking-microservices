using BuildingBlocks.Core.Event;
using BuildingBlocks.PersistMessageProcessor;
using MassTransit;

namespace BuildingBlocks.MassTransit;

// Handle inbox messages with masstransit pipeline
public class ConsumeFilter<T> : IFilter<ConsumeContext<T>>
    where T : class
{
    private readonly IPersistMessageProcessor _persistMessageProcessor;

    public ConsumeFilter(IPersistMessageProcessor persistMessageProcessor)
    {
        _persistMessageProcessor = persistMessageProcessor;
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var id = await _persistMessageProcessor.AddReceivedMessageAsync(
            new MessageEnvelope(
                context.Message,
                context.Headers.ToDictionary(x => x.Key, x => x.Value))
        );

        var message = await _persistMessageProcessor.ExistMessageAsync(id);

        if (message is null)
        {
            await next.Send(context);
            await _persistMessageProcessor.ProcessInboxAsync(id);
        }
    }

    public void Probe(ProbeContext context)
    {
    }
}
